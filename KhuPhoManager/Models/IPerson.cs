using System;
using System.Collections.Generic;

namespace KhuPhoManager.Models
{
    /// <summary>  
    /// Base interface for all person types in the neighborhood  
    /// </summary>  
    public interface IPerson
    {
        /// <summary>  
        /// Gets or sets the person's unique identifier  
        /// </summary>  
        string Id { get; set; }

        /// <summary>  
        /// Gets or sets the person's full name  
        /// </summary>  
        string FullName { get; set; }

        /// <summary>  
        /// Gets or sets the person's age  
        /// </summary>  
        int Age { get; set; }

        /// <summary>  
        /// Gets or sets the person's occupation  
        /// </summary>  
        string Occupation { get; set; }

        /// <summary>  
        /// Gets the type of person (Adult or Child)  
        /// </summary>  
        string PersonType { get; }

        /// <summary>  
        /// Gets or sets the person's date of birth  
        /// </summary>  
        DateTime DateOfBirth { get; set; }
    }
}
