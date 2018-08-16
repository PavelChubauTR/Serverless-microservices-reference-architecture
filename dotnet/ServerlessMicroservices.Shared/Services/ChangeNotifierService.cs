﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerlessMicroservices.Models;
using ServerlessMicroservices.Shared.Helpers;

namespace ServerlessMicroservices.Shared.Services
{
    public class ChangeNotifierService : IChangeNotifierService
    {
        private ISettingService _settingService;
        private ILoggerService _loggerService;

        public ChangeNotifierService(ISettingService setting, ILoggerService logger)
        {
            _settingService = setting;
            _loggerService = logger;
        }

        public async Task DriverChanged(DriverItem driver)
        {
            //TODO: Nothing to do
        }

        public async Task TripCreated(TripItem trip)
        {
            var error = "";

            try
            {
                // Start a trip manager 
                var baseUrl = _settingService.GetStartTripManagerOrchestratorBaseUrl();
                var key = _settingService.GetStartTripManagerOrchestratorApiKey();
                if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(key))
                    throw new Exception("Trip manager orchestrator base URL and key must be both provided");

                await Utilities.Post<dynamic, dynamic>(null, trip, $"{baseUrl}/tripmanagers?code={key}", new Dictionary<string, string>());
            }
            catch (Exception ex)
            {
                error = $"Error while starting the trip manager: {ex.Message}";
                throw new Exception(error);
            }
            finally
            {
                // TODO: Do something with error 
            }
        }

        public async Task TripDeleted(TripItem trip)
        {
            var error = "";

            try
            {
                try
                {
                    // Terminate a trip manager 
                    var baseUrl = _settingService.GetTerminateTripManagerOrchestratorBaseUrl();
                    var key = _settingService.GetTerminateTripManagerOrchestratorApiKey();
                    if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(key))
                        throw new Exception("Trip manager orchestrator base URL and key must be both provided");

                    await Utilities.Post<dynamic, dynamic>(null, trip, $"{baseUrl}/tripmanagers/{trip.Code}/terminate?code={key}", new Dictionary<string, string>());
                }
                catch (Exception)
                {
                    // Report ...but do not re-throw as it is possible not to have a trip manager running
                    //throw new Exception(error);
                }

                try
                {
                    // Terminate a trip monitor 
                    var baseUrl = _settingService.GetTerminateTripMonitorOrchestratorBaseUrl();
                    var key = _settingService.GetTerminateTripMonitorOrchestratorApiKey();
                    if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(key))
                        throw new Exception("Trip monitor orchestrator base URL and key must be both provided");

                    await Utilities.Post<dynamic, dynamic>(null, trip, $"{baseUrl}/tripmonitors/{trip.Code}-M/terminate?code={key}", new Dictionary<string, string>());

                }
                catch (Exception)
                {
                    // Report ...but do not re-throw as it is possible not to have a trip monitor running
                    //throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                // Report ...but do not re-throw as it is possible not to have trip manager or monitor running
                //throw new Exception(error);
            }
            finally
            {
                // TODO: Do something with error 
            }
        }

        public async Task PassengerChanged(PassengerItem trip)
        {
            //TODO: Nothing to do
        }
    }
}
