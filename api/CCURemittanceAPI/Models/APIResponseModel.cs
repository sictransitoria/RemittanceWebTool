using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCURemittanceAPI.Models
{
        public class APIResponseModel
        {
            public APIResponseModel()
            {
                IsSuccess = false;
                Message = "";
            }

            public APIResponseModel(bool isSuccess, string message)
            {
                IsSuccess = isSuccess;
                Message = message;
            }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}