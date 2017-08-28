using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class ReviewDetail : Entity
    {
        public Review Review { get; private set; }
        public ReviewQuestion ReviewQuestion { get; private set; }
        public int Values { get; private set; }

        public ReviewDetail Upate(int values)
        {
            Values = values;
            return this;
        }
        public ReviewDetail(Review review, ReviewQuestion reviewQuestion , int values)
        {
            Review = review;
            ReviewQuestion = reviewQuestion;
            Values = values;
             
        }
        public ReviewDetail()
        {
        }
    }
}
