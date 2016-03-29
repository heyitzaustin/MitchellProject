using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Xml;

namespace MitchellProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : ClaimsService
    {

        public string testSQL(int i)
        {
            string msg = "";
            SqlConnection con = new SqlConnection(String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ClaimsDB.mdf;Integrated Security=True"));
            SqlCommand cmd = new SqlCommand("INSERT INTO TestTable(Id) VALUES(@Id)", con);
            cmd.Parameters.AddWithValue("@Id", i);
            con.Open();
            try {
                int result = cmd.ExecuteNonQuery();
                if (result == 1)
                {
                    msg = "Success!";
                }
                else
                {
                    msg = "Failure";
                }
            }
            catch (Exception e)
            {
                msg = "Exception: " + e;
            }
            con.Close();
            return msg;
        }
        public string CreateClaimFromXML(XmlDocument doc)
        {
            return "";
        }
        public string CreateClaim(Claim claim)
        {
            string msg = "";
            SqlConnection con = new SqlConnection(String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ClaimsDB.mdf;Integrated Security=True"));
            SqlCommand cmd = new SqlCommand("INSERT INTO MitchellClaim(c_number,c_firstname,c_lastname, status, lossdate, adjusterid) VALUES(@c_number, @c_firstname, @c_lastname, @status, @lossdate, @adjusterid)", con);
            cmd.Parameters.AddWithValue("@c_number", claim.ClaimNumber);
            cmd.Parameters.AddWithValue("@c_firstname", claim.FirstName);
            cmd.Parameters.AddWithValue("@c_lastname", claim.LastName);
            cmd.Parameters.AddWithValue("@status", claim.Status);
            cmd.Parameters.AddWithValue("@lossdate", claim.LossDate);
            cmd.Parameters.AddWithValue("@adjusterid", claim.AdjusterId);
            con.Open();

            // Insert into Claims
            try {
                int result = cmd.ExecuteNonQuery();
                if (result == 1)
                {
                    msg = "Success!";
                }
                else
                {
                    msg = "Failure";
                }
            }
            // Insert into vehicles
    

            catch (Exception e)
            {
                msg = "Exception: " + e;
            }
            con.Close();
            return msg;

        }
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
