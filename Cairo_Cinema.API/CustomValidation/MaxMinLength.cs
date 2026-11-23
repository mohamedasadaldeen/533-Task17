using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.API.CustomValidation
{
    public class MaxMinLength : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is string name)
            {
                if (name.Length >=2 && name.Length <= 50)
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}
