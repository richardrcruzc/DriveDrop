
using System.Collections.Generic;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DriveDrop.Api.ViewModels
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    { 
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long Count { get; private set; }
         
        public List<SelectListItem> ListOne { get; private set; }
        public List<SelectListItem> ListTwo { get; private set; }
        public List<SelectListItem> ListThree { get; private set; }


        public IEnumerable<TEntity> Data { get; private set; }

        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data,
             List<SelectListItem> listOne = null, List<SelectListItem> listwo = null, List<SelectListItem> listThree = null)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;

            this.ListOne = listOne;
            this.ListTwo = listwo;
            this.ListThree = listThree;


        }

    }
}
