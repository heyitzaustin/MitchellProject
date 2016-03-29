using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using System.IO;

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

        // Basic temporary Unit Test
        public string testXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Users\\Austin Li\\Documents\\Visual Studio 2015\\Projects\\MitchellProject\\MitchellProject\\create-claim.xml");
            return CreateClaimFromXML(doc.OuterXml);

        }

        public string CreateClaimFromXML(string xmldoc)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmldoc);
            Claim insertclaim = new Claim();

            insertclaim.ClaimNumber = doc.GetElementsByTagName("cla:ClaimNumber")[0].InnerXml;
            insertclaim.FirstName = doc.GetElementsByTagName("cla:ClaimantFirstName")[0].InnerXml;
            insertclaim.LastName = doc.GetElementsByTagName("cla:ClaimantLastName")[0].InnerXml;
            insertclaim.Status = doc.GetElementsByTagName("cla:Status")[0].InnerXml;
            insertclaim.LossDate = doc.GetElementsByTagName("cla:LossDate")[0].InnerXml;
            insertclaim.AdjusterId = Int32.Parse(doc.GetElementsByTagName("cla:AssignedAdjusterID")[0].InnerXml);
            LossInfo lossinfo = new LossInfo();
            lossinfo.cause = doc.GetElementsByTagName("cla:CauseOfLoss")[0].InnerXml;
            lossinfo.reportedDate = doc.GetElementsByTagName("cla:ReportedDate")[0].InnerXml;
            lossinfo.description = doc.GetElementsByTagName("cla:LossDescription")[0].InnerXml;
            insertclaim.lossinfo = lossinfo;
            XmlNodeList vehicles = doc.GetElementsByTagName("cla:VehicleDetails");
            for (int i = 0; i < vehicles.Count; i++)
            {
                XmlNodeList details = vehicles[i].ChildNodes;
                Vehicle tempvehicle = new Vehicle();
                tempvehicle.Vin = details[0].InnerXml;
                tempvehicle.modelYear = details[1].InnerXml;
                tempvehicle.makeDescription = details[2].InnerXml;
                tempvehicle.modelDescription = details[3].InnerXml;
                tempvehicle.engineDescription = details[4].InnerXml;
                tempvehicle.exteriorColor = details[5].InnerXml;
                tempvehicle.licPlate = details[6].InnerXml;
                tempvehicle.licPlateState = details[7].InnerXml;
                tempvehicle.licPlateExpDate = details[8].InnerXml;
                tempvehicle.damageDescription = details[9].InnerXml;
                tempvehicle.mileage = Int32.Parse(details[10].InnerXml);
                insertclaim.vehicles.Add(tempvehicle);
            }

            return CreateClaim(insertclaim);
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
            catch (Exception e)
            {
                msg = "Exception: " + e;
            }
            // Insert into vehicles
            if (claim.vehicles != null)
            {
                foreach (Vehicle ve in claim.vehicles)
                {
                    SqlCommand cmd_ve = new SqlCommand("INSERT INTO Vehicle(" +
                        "vin,c_number,model_year,make_description,model_description,engine_description,exterior_color,lic_plate,lic_plate_state,lic_plate_exp_date,damage_description,mileage) " +
                        "VALUES(@vin, @c_number, @model_year, @make_description, @model_description, @engine_description, @exterior_color, @lic_plate, @lic_plate_state, @lic_plate_exp_date, @damage_description, @mileage)", con);
                    cmd_ve.Parameters.AddWithValue("@vin", ve.Vin);
                    cmd_ve.Parameters.AddWithValue("@c_number", claim.ClaimNumber);
                    cmd_ve.Parameters.AddWithValue("@model_year", ve.modelYear);
                    cmd_ve.Parameters.AddWithValue("@make_description", ve.makeDescription);
                    cmd_ve.Parameters.AddWithValue("@model_description", ve.modelDescription);
                    cmd_ve.Parameters.AddWithValue("@engine_description", ve.engineDescription);
                    cmd_ve.Parameters.AddWithValue("@exterior_color", ve.exteriorColor);
                    cmd_ve.Parameters.AddWithValue("@lic_plate", ve.licPlate);
                    cmd_ve.Parameters.AddWithValue("@lic_plate_state", ve.licPlateState);
                    cmd_ve.Parameters.AddWithValue("@lic_plate_exp_date", ve.licPlateExpDate);
                    cmd_ve.Parameters.AddWithValue("@damage_description", ve.damageDescription);
                    cmd_ve.Parameters.AddWithValue("@mileage", ve.mileage);
                    try
                    {
                        int result = cmd_ve.ExecuteNonQuery();
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
                        Console.WriteLine(e);
                        msg = "Exception: " + e;
                    }
                }
            }
            // Insert Loss Info
            if (claim.lossinfo != null)
            {
                SqlCommand cmd_li = new SqlCommand("INSERT INTO LossInfo(c_number,cause_of_loss,reported_date,loss_description) VALUES" +
                    "(@c_number,@cause_of_loss,@reported_date,@loss_description)", con);
                cmd_li.Parameters.AddWithValue("@c_number", claim.ClaimNumber);
                cmd_li.Parameters.AddWithValue("@cause_of_loss", claim.lossinfo.cause);
                cmd_li.Parameters.AddWithValue("@reported_date", claim.lossinfo.reportedDate);
                cmd_li.Parameters.AddWithValue("@loss_description", claim.lossinfo.description);
                try
                {
                    int result = cmd_li.ExecuteNonQuery();
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
                    Console.WriteLine(e);
                    msg = "Exception: " + e;
                }
            }
            con.Close();
            return msg;

        }

        public string readClaim(string claimnumber)
        {

            Claim returnclaim = new Claim();
            returnclaim.ClaimNumber = claimnumber;
            SqlConnection con = new SqlConnection(String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ClaimsDB.mdf;Integrated Security=True"));
            SqlCommand cmd = new SqlCommand("SELECT * FROM MitchellClaim WHERE c_number='" + claimnumber + "'", con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            // Read from Claims
            if (reader != null)
            {
                if (reader.Read())
                {
                    returnclaim.FirstName = reader["c_firstname"].ToString();
                    returnclaim.LastName = reader["c_lastname"].ToString();
                    returnclaim.Status = reader["status"].ToString();
                    returnclaim.LossDate = reader["lossdate"].ToString();
                    returnclaim.AdjusterId = (int)reader["adjusterid"];
                }
            }
            reader.Close();
            // Read from Vehicles
            SqlCommand cmd_ve = new SqlCommand("SELECT * FROM Vehicle WHERE c_number='" + claimnumber + "'", con);
            reader = cmd_ve.ExecuteReader();

            // Iterate through vehicles
            if (reader != null)
            {
                if (reader.Read())
                {
                    Vehicle vehicle = new Vehicle();
                    vehicle.Vin = reader["vin"].ToString();
                    vehicle.modelYear = reader["model_year"].ToString();
                    vehicle.makeDescription = reader["make_description"].ToString();
                    vehicle.modelDescription = reader["model_description"].ToString();
                    vehicle.engineDescription = reader["engine_description"].ToString();
                    vehicle.exteriorColor = reader["exterior_color"].ToString();
                    vehicle.licPlate = reader["lic_plate"].ToString();
                    vehicle.licPlateState = reader["lic_plate_state"].ToString();
                    vehicle.licPlateExpDate = reader["lic_plate_exp_date"].ToString();
                    vehicle.damageDescription = reader["damage_description"].ToString();
                    vehicle.mileage = (int)reader["mileage"];
                    returnclaim.vehicles.Add(vehicle);
                }
            }
            reader.Close();
            // Read from Loss Info
            SqlCommand cmd_li = new SqlCommand("SELECT * FROM LossInfo WHERE c_number='" + claimnumber + "'", con);
            reader = cmd_li.ExecuteReader();
            if (reader != null)
            {
                if (reader.Read())
                {
                    LossInfo lossinfo = new LossInfo();
                    lossinfo.cause = reader["cause_of_loss"].ToString();
                    lossinfo.reportedDate = reader["reported_date"].ToString();
                    lossinfo.description = reader["loss_description"].ToString();
                    returnclaim.lossinfo = lossinfo;
                }
            }
            // For some reason it won't let me use the ':' character in here.... so instead of cla:MitchellClaim I'll be using MitchellClaim as a name
            XDocument doc = new XDocument(new XElement("MitchellClaim",
                                                new XElement("ClaimNumber", claimnumber),
                                                new XElement("ClaimantFirstName", returnclaim.FirstName),
                                                new XElement("ClaimantLastName", returnclaim.LastName),
                                                new XElement("Status", returnclaim.Status),
                                                new XElement("LossDate", returnclaim.LossDate),
                                                new XElement("LossInfo",
                                                    new XElement("CauseOfLoss", returnclaim.lossinfo.cause),
                                                    new XElement("ReportedDate", returnclaim.lossinfo.reportedDate),
                                                    new XElement("LossDescription", returnclaim.lossinfo.description)),
                                                new XElement("AssignedAdjusterID", returnclaim.AdjusterId),
                                                new XElement("Vehicles",
                                                    returnclaim.vehicles.Select(item => new XElement("VehicleDetails",
                                                                                            new XElement("Vin",item.Vin),
                                                                                            new XElement("ModelYear", item.modelYear),
                                                                                            new XElement("MakeDescription", item.makeDescription),
                                                                                            new XElement("ModelDescription", item.modelDescription),
                                                                                            new XElement("EngineDescription", item.engineDescription),
                                                                                            new XElement("ExteriorColor", item.exteriorColor),
                                                                                            new XElement("LicPlate", item.licPlate),
                                                                                            new XElement("LicPlateState", item.licPlateState),
                                                                                            new XElement("LicPlateExpDate", item.licPlateExpDate),
                                                                                            new XElement("DamageDescription", item.damageDescription),
                                                                                            new XElement("Mileage", item.mileage)
                                                                                            ))
                                                )   
                                                ));
            /*

            //TESTING

            string filename = @"C:\Users\Austin Li\Documents\Visual Studio 2015\Projects\MitchellProject\MitchellProject\OutputXML\temp.xml";
            if (!File.Exists(filename))
            {
                doc.Save(filename);
            }
            */

            // Return xml document as string to be parsed on client end
            return doc.ToString();
        }

        public string deleteClaim(string claimnumber)
        {
            string msg;
            SqlConnection con = new SqlConnection(String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ClaimsDB.mdf;Integrated Security=True"));
            SqlCommand cmd = new SqlCommand("DELETE FROM MitchellClaim WHERE c_number='" + claimnumber + "'", con);
            con.Open();
            try
            {
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
            catch ( Exception e)
            {
                Console.WriteLine(e);
                msg = "Exception: " + e;
            }
            return msg;
            
        }
        public string readVehicleFromClaim(string claimnumber, string vehicle_vin)
        {
            string msg;
            SqlConnection con = new SqlConnection(String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ClaimsDB.mdf;Integrated Security=True"));
            SqlCommand cmd_ve = new SqlCommand("SELECT * FROM Vehicle WHERE c_number='" + claimnumber + "' AND vin='"+vehicle_vin+"'", con);
            con.Open();
            SqlDataReader reader = cmd_ve.ExecuteReader();

            Vehicle vehicle = new Vehicle();
            // Iterate through vehicles
            if (reader != null)
            {
                if (reader.Read())
                {
                    vehicle.Vin = reader["vin"].ToString();
                    vehicle.modelYear = reader["model_year"].ToString();
                    vehicle.makeDescription = reader["make_description"].ToString();
                    vehicle.modelDescription = reader["model_description"].ToString();
                    vehicle.engineDescription = reader["engine_description"].ToString();
                    vehicle.exteriorColor = reader["exterior_color"].ToString();
                    vehicle.licPlate = reader["lic_plate"].ToString();
                    vehicle.licPlateState = reader["lic_plate_state"].ToString();
                    vehicle.licPlateExpDate = reader["lic_plate_exp_date"].ToString();
                    vehicle.damageDescription = reader["damage_description"].ToString();
                    vehicle.mileage = (int)reader["mileage"];
                }
            }
            reader.Close();
            XDocument doc = new XDocument(new XElement("VehicleDetails",
                                       new XElement("Vin", vehicle.Vin),
                                       new XElement("ModelYear", vehicle.modelYear),
                                       new XElement("MakeDescription", vehicle.makeDescription),
                                       new XElement("ModelDescription", vehicle.modelDescription),
                                       new XElement("EngineDescription", vehicle.engineDescription),
                                       new XElement("ExteriorColor", vehicle.exteriorColor),
                                       new XElement("LicPlate", vehicle.licPlate),
                                       new XElement("LicPlateState", vehicle.licPlateState),
                                       new XElement("LicPlateExpDate", vehicle.licPlateExpDate),
                                       new XElement("DamageDescription", vehicle.damageDescription),
                                       new XElement("Mileage", vehicle.mileage)
                                       ));
            // Return XML of Vehicle information
            return doc.ToString();
        }


    }



}
