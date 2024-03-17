﻿using Common.Presentation;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace APIGateway.API
{
    public class PresentationErrors : CommonPresentationErrors
    {
        public new class Validation : CommonPresentationErrors.Validation
        {
            public static class RefreshToken
            {
                public const string InvalidLenght_Code = "Validation.RefreshRoken.InvalidLenght";
            }
        }
    }
}
