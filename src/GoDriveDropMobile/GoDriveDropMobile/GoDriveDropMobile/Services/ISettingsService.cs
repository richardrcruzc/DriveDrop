﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoDriveDrop.Core.Services
{
    public interface ISettingsService
    {
        string AuthAccessToken { get; set; }
        string AuthIdToken { get; set; }
        bool UseMocks { get; set; }
        string UrlBase { get; set; }
        bool UseFakeLocation { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        bool AllowGpsLocation { get; set; }
    }
}
