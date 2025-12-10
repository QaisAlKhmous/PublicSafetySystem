using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PublicSafety.Repositories.Repositories
{
    public class ChangeRequestRepo
    {
        public static void AddNewChangeRequest(ChangeRequest changeRequest)
        {
            using (var context = new AppDbContext())
            {
                context.ChangeRequests.Add(changeRequest);

                context.SaveChanges();
            }
        }

        public static IEnumerable<ChangeRequest> GetAllChangeRequests()
        {
            using(var context = new AppDbContext())
            {
               return context.ChangeRequests.Include(cr => cr.ChangedBy).Include(cr => cr.ApprovedBy).ToList();
            }
        }

        public static ChangeRequest GetChangeRequestById(Guid ChangeRequestId)
        {
            using (var context = new AppDbContext())
            {
                return context.ChangeRequests.Include(cr => cr.ChangedBy)
                    .Include(cr => cr.ApprovedBy)
                    .FirstOrDefault(cr => cr.RequestId == ChangeRequestId);
            }
        }

        public static void UpdateChangeRequest(ChangeRequest request)
        {
            using (var context = new AppDbContext())
            {
                var updatedRequest = context.ChangeRequests.Find(request.RequestId);

                updatedRequest.AdminComment = request.AdminComment;
                updatedRequest.ApprovedById = request.ApprovedById;
                updatedRequest.ApprovedDate = request.ApprovedDate;
                updatedRequest.Status = request.Status;

                context.SaveChanges();
            }
        }
    }
}
