namespace ComplaintManagement.Helpers
{
    public class Messages
    {
        public const string FAIL = "Fail";
        public const string SUCCESS = "Success";
        public const string ALREADY_EXISTS = "Already exists";

        public const string NormalUser = "Normal";
        public const string HRUser = "HR";
        public const string AdminUser = "Admin";

        public const string BAD_DATA = "Bad or Invalid data";
        public const string USER_EXISTS = "User already exists";
        public const string LOGIN_TYPE_FACEBOOK = "Facebook";
        public const string LOGIN_TYPE_LOCAL = "Local";
        public const string WORK_EMAIL_EXIST = "Work email is already exist.";
        public const string EMP_ID_EXIST = "Employee Id is already exist.";
        public const string RoleMasterComplaintCriteriaNotFound = "Role master master is not defined in given criteria with LOS,SUBSBU,SBU,Competency.";
        public const string ComplaintSubmitted = "Complaint has been submitted.";

        public const string USER_ALREADY_REWARDED = "User is already rewarded";
        public const string USER_DONT_HAVE_ENOUGH_AMOUNT = "User have not wallet amount in his account";
        public const string INVALID_USER_PASS = "Invalid email or password";
        public const string FORGOTPASSEMAIL = "Invalid email.";
        public const string NOT_ACTIVE = "Account not active";
        public const string LESS_THEN_12_HOURS = "You are trying to update order after 12 hours, please contact to admin from message box regarding your order with order reference id {0}";

        public const string ERROR_SENDING_EMAIL = "Error sending email";

        public const string PASSWORD_RESET = "Reset Password Request";
        public const string PASSWORD_RESET_SUCCESS = "Please check your email for reset password";

        public const string PASSWORD_RESET_MESSAGE = "Hello,<br/><br/>We received a request to reset the password for your Account {0}.<br/><br/>Please click on the following link <a href= '{1}/Account/resetpassword?token={2}' target= '_blank' >{1}/Account/resetpassword?token={2}</a> <br/><br/>Please contact us if you have any problems with your login.<br/><br/>Thank you";

        public const string FORGET_PASSWORD_RESET_MESSAGE = "Hello,<br/><br/>We received a request to reset the password for your Account {0}.<br/><br/>Please click on the following link <a href= 'https://www.pithplay.com/lostpassword/{1}' target= '_blank' >https://www.pithplay.com/lostpassword/{1}</a> <br/><br/>For your security, this link will expire in 24 hours or after your password has been reset.<br/><br/>Please contact us if you have any problems with your login.<br/><br/>Thank you";


        public const string CURRENT_PASSWORD_MESSAGE = "Current password is wrong";

        public const string ADD_MESSAGE = "Successfully added {0} detail";

        public const string UPDATE_MESSAGE = "Successfully updated {0} detail";

        public const string DELETE_MESSAGE = "Successfully deleted {0} detail";
        public const string DELETE_MESSAGE_FILE = "Successfully deleted file.";

        public const string COMMENT_APPROVED_MESSAGE = "Successfully published {0} detail";
        public const string Added = "Added";
        public const string Updated = "Updated";

        public const string Opened = "Opened";
        public const string Closed = "Closed";
        public const string Withdrawn = "Withdrawn";

        public const string PRODUCT_IMAGE = "Product";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Status = "Status";
        public const string Id = "Id";

        public const string DataCategoryAlreadyExists = "This Category {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataCommitteeAlreadyExists = "This Committee {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataCompetencyAlreadyExists = "This Competency {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataDesignationAlreadyExists = "This Designaton {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataRegionAlreadyExists = "This Region {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataEntityAlreadyExists = "This Entity {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataLocationAlreadyExists = "This Location {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataSBUAlreadyExists = "This SBU {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataSubSBUAlreadyExists = "This SubSBU {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataSubCategoryAlreadyExists = "This SubCategory {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataLOSAlreadyExists = "This SubCategory {0} already exists! - Row No. {1} and Column No. {2}";
        public const string DataRoleAlreadyExists = "This SubCategory {0} already exists! - Row No. {1} and Column No. {2}";


        public const string DataEmpAlreadyExists = "This Employee {0} already exists! - Row No. {1} and Column No. {2}";
        public const string FieldCannotBeBlankOrEmpty = "{0} cannot be blank or contain white space.";
        public const string FieldIsRequired = "{0} is required - Row No. {1} and Column No. {2}";
        public const string GenderInvalid = "Please select gender Male or Female - Row No. {1} and Column No. {2}";
        public const string TimeTypeInvalid = "Please select time type part time or full time - Row No. {1} and Column No. {2}";
        public const string ManagerInvalid = "Please select time type normal or admin or hr - Row No. {1} and Column No. {2}";
        public const string TypeInvalid = "Please select time type normal or admin or hr - Row No. {1} and Column No. {2}";
        public const string StatusInvalid = "Please select status active or inactive - Row No. {1} and Column No. {2}";
        public const string EmpManagerPattern = "Employee ID or name or email";
        public const string DataEmpNOTExists = "{0} not exists! - Row No. {1} and Column No. {2}";
        public const string FieldIsInvalid = "{0} should not contain special characters - Row No. {1} and Column No. {2}";

        public const string DataNOTExists = "{0} not exists! - Row No. {1} and Column No. {2}";
        public const string EmailFormat = "Please work email in email format - Row No. {0} and Column No. {1}";
        public const string AgeInvalid = "Please select age in digit only - Row No. {0} and Column No. {1}";
        public const string DOJInvalid = "Please enter valid date of joining - Row No. {0} and Column No. {1}";
        public const string MobileNumberInvalid = "Please enter valid mobile number - Row No. {0} and Column No. {1}";
        public const string MobileNumberInvalidFormat = "Mobile number should not contain char - Row No. {0} and Column No. {1}";

        public const string LOGO = "Logo";

        public const string ORDER_NOT_REVISION_YET = "Order Not Delivered";
        public const string EmployeeName = "Employee Name";
        public const string TimeType = "Time Type";
        public const string BusinessTitle = "Business Title";

        public const string Company = "Company";
        public const string Manager = "Manager";

        public const string CaseStage = "CaseStage";

        public const string NotAvailable = "N/A";
        public const string ABOUT_ORDER_NO = "ABOUT YOUR ORDER {0}";
        public const string ORDER_NEW_NOTIFICATION_USER = "Sub-Order Id-{0}";
        public const string ORDER_UPDATE_NOTIFICATION_USER = "SUB- ABOUT YOUR ORDER ID--{0}";
        public const string ORDER_SUBMITTED_MESSAGE = "Hello,<br/><br/> We have received your order request. Our Admin will respond about your order very soon. <br/><br/> Thanks.";
        public const string ORDER_NEW_ADMIN_SUBMITTED_MESSAGE = "Hello,<br/><br/> We have received new order request id {0}. User : {1}. <br/><br/> Thanks.";

        public const string ORDER_UPDATED_MESSAGE = "Hello,<br/><br/> We have received your order request. Our Admin will respond about your order very soon. <br/><br/> Thanks.";
        public const string ORDER_COMPLETE_MESSAGE = "Hello,<br/><br/>Thank you for buying your product. <br/><br/> We have completed your order. <br/><br/> Thanks.";
        public const string ORDER_CANCEL_MESSAGE = "Hello,<br/><br/>We have cancelled your order as per your request. <br/><br/> Your $ {0} refund will be credited to dashboard as soon as possible in 3-4 bussiness day. <br/><br/> Thanks.";
        public const string ORDER_REFUNDED_MESSAGE = "Hello,<br/><br/>We have cancelled your order as per your request. <br/><br/> Your $ {0} refund credited to dashboard. <br/><br/> Thanks.";
        public const string ORDER_STATUS = "Hello,<br/><br/> We have received your order request. We have {0} your order. <br/><br/> Thanks.";
        public const string ORDER_UPDATED = "Order Updated of {0}";
        public const string ORDER_UPDATE_STATUS = "Hello,<br/><br/> We have received order update request order id of {0}. <br/><br/> Thanks.";
        public const string ORDER_UPDATESCRIPT_STATUS = "Hello,<br/><br/> We have received script updated order update request order id of {0}. <br/><br/> Please check previous file along with order id<br/><br/> Thanks.";
        public const string NOT_EMAIL_VERIFIED = "Account is not email verified";
        public const string NEW_USERREGISTRATION_MESSAGE = "Hello,<br/><br/>Thank you so much for registering with pithplay.com as {0}.<br/><br/>Please click on the following link to confirm your account link : <a href= '{1}/Account/confirmemail?token={2}' target= '_blank' >{1}/Account/confirmemail?token={2}</a> <br/><br/>Please contact us if you have any problems with your login.<br/><br/>Thank you";
        public const string ACCOUNT_ACTIVATION = "About your account activation from pithplay.com";
        //Order statuses

        public const string SUBMITTED = "Submitted";
        public const string COMMITTEE = "Committee";
        public const string ORDERED = "ordered";
        public const string WORKING = "working";
        public const string DELIVERED = "delivered";
        public const string REVISION = "revision";
        public const string REFUNDED = "refunded";
        public const string CANCELLED = "cancelled";
        public const string COMPLETED = "Completed";
        public const string XLSX = ".xlsx";
        public const string InProgress = "In-Progress";

        //Image paths
        public const string LOGOIMG = "logo";
        public const string PRODUCT = "product";
        public const string AUDIOFILE = "audiofile";
        public const string SCRIPTIMAGES = "scriptimages";

        public const string CreatedDate = "Created Date";
        public const string ModifiedDate = "Modified Date";
        public const string ModifiedBy = "Modified By";
        public const string CreatedBy = "Created By";
        public const string Remark = "Remark";
        public const string PendingWith = "Pending With";
        public const string ComplaintList = "ComplaintList";

        public const string Category = "Category";
        public const string CategoryHistory = "Category History";

        public const string SubCategory = "SubCategory";
        public const string SubCategoryHistory = "SubCategory History";

        public const string Designation = "Designation";
        public const string DesignationHistory = "Designation History";

        public const string LOS = "LOS";
        public const string LOSHistory = "LOS History";
        public const string InvolvedUser = "InvolvedUser";

        public const string SBU = "SBU";
        public const string SBUHistory = "SBU History";

        public const string SubSBU = "SubSBU";
        public const string SubSBUHistory = "SubSBU History";

        public const string Region = "Region";
        public const string RegionHistory = "Region History";

        public const string Location = "Location";
        public const string LocationHistory = "Location History";

        public const string Competency = "Competency";
        public const string CompetencyHistory = "Competency History";

       

        public const string Entity = "Entity";
        public const string EntityHistory = "Entity History";

        public const string Role = "Role";
        public const string RoleHistory = "Role History";

        public const string Committee = "Committee";
        public const string CommitteeHistory = "Committee History";

        public const string UserMaster = "User Master";
        public const string UserMasterHistory = "User Master History";

        public const string User = "User";
        public const string UserName = "User Name";

        public const string EntityState = "Entity State";

    }
}