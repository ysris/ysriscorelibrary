using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary
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

    public static class Role
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
        public const string CompanyAdministrator = "CompanyAdministrator";
        public const string Proprietaire = "Proprietaire";
        public const string Coach = "Coach";
        public const string Locataire = "Locataire";
        public const string Business = "Business";
    }
}
