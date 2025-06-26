using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace modelValidationExample.CustomValidators
{
    public class DateRangeValidatorAttribute : ValidationAttribute
    {
        private string OtherPropertyName { get; set; }

        public DateRangeValidatorAttribute(string otherPropertyName)
        {
            OtherPropertyName = otherPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime to_date = (DateTime)value;

                //*********--Reflection in c# --*********//
                // to get value of other property (here is FromDate) as we have only name of it we use reflection concept
                // get type of object the have this property (person) then call get property on type with name to get property info 
                // of propert (fromdate) 
                // then with this property info we can read (get) or update (set) values 
                // in this case we use getvalue to get the value of property (fromdate) and give this method the obj that 
                // we get from it the property value
                //******************//
                PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(OtherPropertyName);

                if (otherProperty != null)
                {
                    DateTime from_Date = Convert.ToDateTime(otherProperty.GetValue(validationContext.ObjectInstance));

                    if (from_Date > to_date)
                    {
                        return new ValidationResult(ErrorMessage, new string[] { OtherPropertyName, validationContext.MemberName });
                    }
                    else return ValidationResult.Success;
                }
                else return null;
            }
            return null;
        }
    }
}
