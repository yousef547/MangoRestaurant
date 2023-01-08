﻿namespace Mango.Services.ProductAPI.Model
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMassage { get; set; }
        public List<string> ErrorMessages { get; set; }

    }
}
