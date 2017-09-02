using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Enums
{
    public enum CustomerStatus
    {
        Disabled,
        PendingActivationWithPasswordChange,
        PendingActivationWithoutPasswordChange,
        Activated,        
        PendingOnBoarding, //User has to fill an onboarding step 
        PendingOnBoardingCompleted //User has filled onboarding and need validation from team before being Activated
    }
}
