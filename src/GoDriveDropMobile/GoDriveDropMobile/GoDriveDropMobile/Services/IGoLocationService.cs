//
//  IWalkLocationService.cs
//  TrackMyWalks Location Service Interface
//
//  Created by Steven F. Daniel on 16/09/2016.
//  Copyright © 2016 GENIESOFT STUDIOS. All rights reserved.
//
using System;

namespace GoDriveDrop.Core.Services
{
	// Define our Walk Location Service Interface
	public interface IGoLocationService
	{
		// Define our Location Service Instance Methods
		void GetMyLocation();

		double GetDistanceTravelled(double lat, double lon);
		event EventHandler<IGoCoordinates> MyLocation;
	}

	// Go Location Coordinates Obtained
	public interface IGoCoordinates
	{
		double Latitude { get; set; }
		double Longitude { get; set; }
	}
}