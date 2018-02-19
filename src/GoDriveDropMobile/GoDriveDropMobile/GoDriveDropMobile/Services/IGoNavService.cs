//
//  IWalkNavService.cs
//  TrackMyWalks Navigation Service Interface
//
//  Created by Steven F. Daniel on 03/09/2016.
//  Copyright © 2016 GENIESOFT STUDIOS. All rights reserved.
//
using System.Threading.Tasks;
using GoDriveDrop.Core.ViewModels;

namespace GoDriveDrop.Core.Services
{
	public interface IGoNavService
	{
		// Navigate back to the Previous page in the NavigationStack
		Task PreviousPage();

		// Navigate to the first page within the NavigationStack 
		Task BackToMainPage();

		// Navigate to a particular ViewModel within our MVVM Model,
		// and pass a parameter
		Task NavigateToViewModel<ViewModel, WalkParam>(WalkParam parameter)
			where ViewModel : BaseViewModel;

		// Clear all previously created views from the NavigationStack
		void ClearAllViewsFromStack();
	}
}