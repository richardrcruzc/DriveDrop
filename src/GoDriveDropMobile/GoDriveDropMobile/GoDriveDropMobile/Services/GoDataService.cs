////
////  WalkDataService.cs
////  TrackMyWalks API Data Service Class
////
////  Created by Steven F. Daniel on 30/10/2016.
////  Copyright © 2016 GENIESOFT STUDIOS. All rights reserved.
////
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Net.Http;
//using GoDriveDrop.Core.Models;
//using Newtonsoft.Json;

//namespace GoDriveDrop.Core.Services
//{
//	public class GoDataService : GoWebService, IGoDataService
//	{
//		readonly Uri _baseUri;
//		readonly IDictionary<string, string> _headers;

//		// Our Class Constructor that accepts the Azure Database
//		// Uri path
//		public GoDataService(Uri baseUri, string authToken)
//		{
//			_baseUri = baseUri;
//			_headers = new Dictionary<string, string>();
//			_headers.Add("zumo-api-version", "2.0.0");
//		}

//		// API to retrieve our Walk Entries from our database
//		public async Task<IList<string>> GetstringAsync()
//		{
//			var url = new Uri(_baseUri, "/tables/string");
//			return await SendRequestAsync<string[]>(url, HttpMethod.Get, _headers);
//		}

//		// API to add our Walk Entry information to the database
//		public async Task AddWalkEntryAsync(string entry)
//		{
//			var url = new Uri(_baseUri, "/tables/string");
//			await SendRequestAsync<string>(url, HttpMethod.Post, _headers, entry);
//		}

//		// API to delete our Walk Entry from the database
//		public async Task DeleteWalkEntryAsync(string entry)
//		{
//			var url = new Uri(_baseUri, string.Format("/tables/string/{0}", entry));
//			await SendRequestAsync<string>(url, HttpMethod.Delete, _headers);
//		}
//	}
//}