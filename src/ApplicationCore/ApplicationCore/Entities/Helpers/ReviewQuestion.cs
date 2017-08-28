using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
    public class ReviewQuestion : Entity
    {

        public string Group { get; private set; }
        public string Description { get; private set; }
         
        public ReviewQuestion(string group, string description)
        {
            Group = group;
            Description = description;
        }

        public ReviewQuestion()
        { }
    }
}
