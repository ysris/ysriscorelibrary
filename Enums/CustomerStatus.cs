using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Enums
{
    public static class CustomerStatus
    {
        public const string Disabled = "Disabled";
        public const string PendingActivationWithPasswordChange = "PendingActivationWithPasswordChange";
        public const string PendingActivationWithoutPasswordChange = "PendingActivationWithoutPasswordChange";
        public const string Activated = "Activated";

		//User has to fill an onboarding step 
		public const string PendingOnBoarding = "PendingOnBoarding";

		//User has filled onboarding and need validation from team before being Activated
		public const string PendingOnBoardingCompleted = "PendingOnBoardingCompleted";
    }
}
