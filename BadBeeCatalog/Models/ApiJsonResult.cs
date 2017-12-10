using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BadBeeCatalog.Models
{
    [Serializable]
    public class ApiJsonResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }

        /// <summary>
        /// Empty constructor for ApiJsonResult.
        /// </summary>
        public ApiJsonResult() { }

        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="isSuccess">If the API request was a success or not.</param>
        /// <param name="message">The message to describe the request result.</param>
        public ApiJsonResult(Boolean isSuccess, String message) {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.Object = null;
        }

        /// <summary>
        /// In this contructor, we assume that the message is to inform of an error.
        /// </summary>
        /// <param name="message">Error mesage.</param>
        public ApiJsonResult(object obj)
        {
            if (obj != null)
            {
                this.IsSuccess = true;
                this.Message = null;
            }
            else
            {
                this.IsSuccess = false;
                this.Message = "Null object found"; ;
            }

            this.Object = obj;
        }

    }
}
