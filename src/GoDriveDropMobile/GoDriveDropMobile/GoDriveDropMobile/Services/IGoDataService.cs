//
//  IWalkDataService.cs
//  TrackMyWalks Data Service Interface
//
//  Created by Steven F. Daniel on 30/10/2016.
//  Copyright © 2016 GENIESOFT STUDIOS. All rights reserved.
//
using System.Threading.Tasks;
using System.Collections.Generic;
using GoDriveDrop.Core.Models;

namespace GoDriveDrop.Core.Services
{
	public interface IGoDataService
	{
		Task<IList<string>> GetstringAsync();
		Task AddWalkEntryAsync(string entry);
		Task DeleteWalkEntryAsync(string entry);
	}
}