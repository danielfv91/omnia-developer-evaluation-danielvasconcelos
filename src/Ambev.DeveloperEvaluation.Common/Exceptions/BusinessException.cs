﻿using System;

namespace Ambev.DeveloperEvaluation.Common.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}