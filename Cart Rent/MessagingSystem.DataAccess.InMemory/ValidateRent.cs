using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingSystem.DataAccess.InMemory
{
    class ValidateRent
    {
        private DatabaseEntity db = new DatabaseEntity();

        private bool validateCar(Reservations reservations)
        {
            string sDate = reservations.StartDate.ToString("yyyy-MM-dd");
            string eDate = reservations.EndDate.ToString("yyyy-MM-dd");

            var carList = db.Database.SqlQuery<int>("SELECT DISTINCT ReservationID FROM Reservations WHERE CarID = @carID" +
                " and ReservationID IN (SELECT ReservationID FROM Reservations WHERE NOT ((StartDate > @endDate) OR (EndDate < @startDate)))"
                , new SqlParameter("carID", reservations.CarID), new SqlParameter("endDate", eDate), new SqlParameter("startDate", eDate)).ToList();

            if (carList.FirstOrDefault() > 0)
            {
                return false;
            }
            return true;
        }

        private bool validateCustomer(int id)
        {
            if (db.Customers.Find(id) == null)
            {
                return false;
            }

            return true;
        }

        private bool validateLocation(string location, int carID)
        {
            var carAtLocation = db.Database.SqlQuery<int>("Select CarID FROM Cars WHERE Location = @location", new SqlParameter("location", location)).ToList<int>(); ;
            if (carAtLocation.Contains(carID))
            {
                return true;
            }
            return false;
        }
    }
}
