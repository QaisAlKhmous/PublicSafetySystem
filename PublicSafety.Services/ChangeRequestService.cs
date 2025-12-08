using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class ChangeRequestService
    {
        public static void AddNewChangeRequest(ChangeRequestDTO changeRequest)
        {
            var newChangeRequest = new ChangeRequest()
            {
                RequestId = Guid.NewGuid(),
                OldValue = changeRequest.OldValue,
                NewValue = changeRequest.NewValue,
                AdminComment = changeRequest.AdminComment,
                ChangedById = UserService.GetUserByUsername(changeRequest.ChangedBy).UserId,
                RequestDate = DateTime.Now,
                EntityId = changeRequest.EntityId,
                Status = enStatus.pending,
                EntityType = (enEntityType)Enum.Parse(typeof(enEntityType), changeRequest.EntityType),
               

            };
           ChangeRequestRepo.AddNewChangeRequest(newChangeRequest);
        }

        public static IEnumerable<ChangeRequestDTO> GetAllChangeRequests()
        {
            return ChangeRequestRepo.GetAllChangeRequests().Select(cr => new ChangeRequestDTO
            {
                RequestId = cr.RequestId,
                OldValue = cr.OldValue,
                NewValue = cr.NewValue,
                AdminComment = cr.AdminComment,
                ChangedBy = cr.ChangedBy.Username,
                EntityId = cr.EntityId,
                EntityType = cr.EntityType.ToString(),
                RequestDate = cr.RequestDate.ToString(),
                Status = cr.Status.ToString()
            });
        }
    }
}
