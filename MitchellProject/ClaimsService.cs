using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MitchellProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ClaimsService
    {


        [OperationContract]
        string CreateClaim(Claim claim);

        [OperationContract]
        string CreateClaimFromXML(string xmlddoc);

        // Testing operation
        [OperationContract]
        string testSQL(int i);

        // Testing operation
        [OperationContract]
        string testXML();

        [OperationContract]
        string readClaim(string claimnumber);

        [OperationContract]
        string deleteClaim(string claimnumber);
        // TODO: Add your service operations here

        [OperationContract]
        string readVehicleFromClaim(string claimnumber, string vehicle_vin);

        [OperationContract]
        List<string> getClaimsInDateRange(string startdate, string enddate);
    }



    [DataContract]
    public class Claim
    {
        string claimNumber = String.Empty;
        string firstName = String.Empty;
        string lastName = String.Empty;
        string status = String.Empty;
        string lossdate = String.Empty;
        int adjusterId = 0;
        public List<Vehicle> vehicles = new List<Vehicle>();
        public LossInfo lossinfo = new LossInfo();

        [DataMember]
        public string ClaimNumber
        {
            get { return claimNumber; }
            set { claimNumber = value; }
        }
        [DataMember]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        [DataMember]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        [DataMember]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        [DataMember]
        public string LossDate
        {
            get { return lossdate; }
            set { lossdate = value; }
        }
        [DataMember]
        public int AdjusterId
        {
            get { return adjusterId; }
            set { adjusterId = value; }
        }
    }
}
