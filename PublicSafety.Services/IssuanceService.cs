using DocumentFormat.OpenXml.Spreadsheet;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class IssuanceService
    {

        public static void AddNewIssuance(AddIssuanceDTO issuance)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var user = UserService.GetUserByUsername(issuance.CreatedBy);
                    var employee = EmployeeRepo.GetEmployeeById(issuance.EmployeeId);

                    var issuanceType =
                        (enIssuanceType)Enum.Parse(typeof(enIssuanceType), issuance.Type);

                    var newIssuance = new Issuance
                    {
                        IssuanceId = Guid.NewGuid(),
                        EmployeeId = issuance.EmployeeId,
                        ItemId = issuance.ItemId,
                        Quantity = issuance.Quantity,

                        IssuanceDate = issuanceType == enIssuanceType.Entitled
                            ? new DateTime(int.Parse(issuance.IssuanceDate), 1, 1)
                            : DateTime.Now,

                        Type = issuanceType,
                        ExceptionReason = issuance.ExceptionReason ?? string.Empty,
                        ExceptionFormPath = issuance.ExceptionFormPath,
                        SignedReceiptPath = issuance.SignedReceiptPath,

                        CreatedById = user.UserId,
                        CreatedDate = DateTime.Now,

                        MatrixItemId = issuanceType == enIssuanceType.Entitled
                            ? issuance.MatrixItemId
                            : null
                    };

                    // 1️⃣ Add issuance
                    context.Issuances.Add(newIssuance);

                    // 2️⃣ Decrease stock (ALL types)
                    var item = context.Items.Find(issuance.ItemId);
                    if (item == null)
                        throw new Exception("Item not found");

                    if (item.Quantity < issuance.Quantity)
                        throw new Exception("Insufficient stock");

                    item.Quantity -= issuance.Quantity;

                    // 3️⃣ Add item log
                    var log = new ItemLog
                    {
                        ItemLogId = Guid.NewGuid(),
                        EmployeeId = issuance.EmployeeId,
                        ItemId = issuance.ItemId,
                        MatrixItemId = newIssuance.MatrixItemId,
                        IssuanceId = newIssuance.IssuanceId,

                        ActionType =
        issuanceType == enIssuanceType.Entitled
            ? enItemActionType.Entitled
            : issuanceType == enIssuanceType.Exception
                ? enItemActionType.Exception
                : enItemActionType.Damaged,

                        Quantity = issuance.Quantity,

                        EntitlementYear =
        issuanceType == enIssuanceType.Entitled
            ? int.Parse(issuance.IssuanceDate)
            : (int?)null,

                        Notes = issuanceType == enIssuanceType.Entitled
        ? $"صرف استحقاق للموظف {employee.FullName}"
        : issuanceType == enIssuanceType.Exception
            ? $"صرف استثنائي للموظف {employee.FullName}. السبب: {issuance.ExceptionReason}"
            : $"استبدال صنف تالف للموظف {employee.FullName}",

                        CreatedById = user.UserId,
                        CreatedDate = DateTime.Now
                    };


                    context.ItemLogs.Add(log);

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        static string ToYearString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Case 1: already a year (e.g. "2024")
            if (int.TryParse(value, out int year) && year >= 1900 && year <= 2100)
                return year.ToString();

            // Case 2: full date / ISO / UTC
            if (DateTimeOffset.TryParse(value, out var dto))
                return dto.UtcDateTime.Year.ToString();

            return null; // invalid
        }
        public static void AddNewEntitledIssuance(AddIssuanceDTO issuance)
        {
            issuance.IssuanceDate = ToYearString(issuance.IssuanceDate);
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var userId = UserService
                        .GetUserByUsername(issuance.CreatedBy)
                        .UserId;

                    var newIssuance = new Issuance
                    {
                        IssuanceId = Guid.NewGuid(),
                        EmployeeId = issuance.EmployeeId,
                        ItemId = issuance.ItemId,
                        MatrixItemId = issuance.MatrixItemId,
                        Quantity = issuance.Quantity,


                        IssuanceDate = DateTime.Parse("01-01-" + issuance.IssuanceDate),

                        Type = enIssuanceType.Entitled,
                        ExceptionReason = issuance.ExceptionReason ?? string.Empty,
                        ExceptionFormPath = issuance.ExceptionFormPath,
                        SignedReceiptPath = issuance.SignedReceiptPath,
                        CreatedById = userId,
                        CreatedDate = DateTime.Now
                    };

                    // 1️⃣ Save issuance
                    IssuanceRepo.AddIssuance( newIssuance);

                    // 2️⃣ Decrease stock
                    ItemRepo.DecreaseItemQuantity(
                        issuance.ItemId,
                        issuance.Quantity
                    );
                    var employee = context.Employees.Find(issuance.EmployeeId);
                    if (employee == null)
                        throw new Exception("الموظف غير موجود");

                    ItemLogRepo.Add(context, new ItemLog
                    {
                        ItemLogId = Guid.NewGuid(),
                        EmployeeId = issuance.EmployeeId,
                        ItemId = issuance.ItemId,
                        MatrixItemId = issuance.MatrixItemId,
                        IssuanceId = newIssuance.IssuanceId,

                        ActionType = enItemActionType.Entitled,
                        Quantity = issuance.Quantity,
                        EntitlementYear = DateTime.Parse("01-01-" + issuance.IssuanceDate).Year,

                        Notes = $"صرف استحقاق للموظف {employee.FullName} لسنة {DateTime.Parse("01-01-" + issuance.IssuanceDate).Year}",

                        CreatedById = userId,
                        CreatedDate = DateTime.Now
                    });


                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public static IEnumerable<IssuanceDTO> GetIssuancesByEmployeeId(Guid EmployeeId)
        {
            var issuances = IssuanceRepo.GetIssuancesByEmployeeId(EmployeeId);

            return issuances.Select(i => new IssuanceDTO()
            {
                Item = i.Item.Name,
                CreatedBy = i.CreatedBy.Username,
                Quantity = i.Quantity,
                ExceptionReason = i.ExceptionReason,
                ExceptionFormPath = i.ExceptionFormPath,
                IssuanceDate = i.IssuanceDate.ToString("yyyy-MM-dd"),
                SignedReceiptPath = i.SignedReceiptPath,
                Type = i.Type.ToString(),
                CreatedDate = i.CreatedDate.ToString("yyyy-MM-dd"),
               
            });
        }


        public static void IssueMatrixForCategory(Guid categoryId, int year,Guid UserId,string SignedReceiptPath)
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction())
            {
                try
                {
                    var matrix = context.Matrices
                        .SingleOrDefault(m => m.CategoryId == categoryId && m.IsActive);

                    if (matrix == null)
                        throw new Exception("لا توجد مصفوفة فعالة لهذه الفئة");

                    var matrixItems = context.MatrixItems
                        .Where(mi => mi.MatrixId == matrix.MatrixId)
                        .ToList();

                    if (!matrixItems.Any())
                        throw new Exception("المصفوفة لا تحتوي على أصناف");

                    var periods =
                        (from h in context.EmployeeJobTitleHistories
                         join jtc in context.JobTitleCategories
                             on h.JobTitleId equals jtc.JobTitleId
                         where jtc.CategoryId == categoryId
                         select new
                         {
                             h.EmployeeId,
                             StartYear = h.StartDate.Year,
                             EndYear = (h.EndDate ?? DateTime.Now).Year
                         }).ToList();

                    var requiredPerItem = new Dictionary<Guid, int>();

                    foreach (var p in periods)
                    {
                        if (year < p.StartYear || year > p.EndYear)
                            continue;

                        foreach (var mi in matrixItems)
                        {
                            
                            if ((year - p.StartYear) % mi.Frequency != 0)
                                continue;

                          
                            int issuedQty = context.Issuances
                                .Where(i =>
                                    i.EmployeeId == p.EmployeeId &&
                                    i.MatrixItemId == mi.MatrixItemId &&
                                    i.IssuanceDate.Year == year &&
                                    i.Type == enIssuanceType.Entitled)
                                .Sum(i => (int?)i.Quantity) ?? 0;

                            int remainingQty = mi.Quantity - issuedQty;

                            if (remainingQty <= 0)
                                continue;

                            if (!requiredPerItem.ContainsKey(mi.ItemId))
                                requiredPerItem[mi.ItemId] = 0;

                            requiredPerItem[mi.ItemId] += remainingQty;
                        }
                    }

                   
                    foreach (var kv in requiredPerItem)
                    {
                        var stockItem = context.Items.Find(kv.Key);

                        if (stockItem == null)
                            throw new Exception("صنف غير موجود");

                        if (stockItem.Quantity < kv.Value)
                            throw new Exception($"المخزون غير كافٍ للصنف {stockItem.Name}");
                    }

                   
                    DateTime issuanceDate = new DateTime(year, 1, 1);

              
                    foreach (var p in periods)
                    {
                        if (year < p.StartYear || year > p.EndYear)
                            continue;

                        foreach (var mi in matrixItems)
                        {
                            if ((year - p.StartYear) % mi.Frequency != 0)
                                continue;

                            int issuedQty = context.Issuances
                                .Where(i =>
                                    i.EmployeeId == p.EmployeeId &&
                                    i.MatrixItemId == mi.MatrixItemId &&
                                    i.IssuanceDate.Year == year &&
                                    i.Type == enIssuanceType.Entitled)
                                .Sum(i => (int?)i.Quantity) ?? 0;

                            int remainingQty = mi.Quantity - issuedQty;

                            if (remainingQty <= 0)
                                continue;

                           
                            context.Issuances.Add(new Issuance
                            {
                                IssuanceId = Guid.NewGuid(),
                                EmployeeId = p.EmployeeId,
                                ItemId = mi.ItemId,
                                MatrixItemId = mi.MatrixItemId,
                                Quantity = remainingQty,
                                IssuanceDate = issuanceDate,  
                                CreatedDate = DateTime.Now,    
                                Type = enIssuanceType.Entitled,
                                CreatedById = UserId,
                                SignedReceiptPath = SignedReceiptPath
                            });

                           
                            var stockItem = context.Items.Find(mi.ItemId);
                            stockItem.Quantity -= remainingQty;
                        }
                    }

                    context.SaveChanges();
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public static void AttachSignedReceipt(
    Guid employeeId,
    int entitlementYear,
    string receiptPath)
        {
            using (var context = new AppDbContext())
            {
                var issuances = context.Issuances
                    .Where(i =>
                        i.EmployeeId == employeeId &&
                        i.Type == enIssuanceType.Entitled &&
                        i.SignedReceiptPath == null &&
                        i.IssuanceDate.Year == entitlementYear
                    )
                    .ToList();

                if (!issuances.Any())
                    throw new Exception("لا توجد عمليات صرف بحاجة إلى إيصال لهذه السنة");

                foreach (var issuance in issuances)
                {
                    issuance.SignedReceiptPath = receiptPath;
                }

                context.SaveChanges();
            }
        }


        public static void IssueEmployeeEntitlementsForYear(
    IssueEmployeeYearDTO yearIssuance)
        {
            var entitlements = EntitlementRepo
                .GetEmployeeEntitlemenets(yearIssuance.EmployeeId)
                .Where(e =>
                    e.EntitlementYear == yearIssuance.Year &&
                    e.RemainingQty > 0)
                .ToList();

            if (!entitlements.Any())
                throw new Exception("لا يوجد استحقاقات قابلة للصرف لهذه السنة");

            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var issuances = new List<Issuance>();

                    foreach (var e in entitlements)
                    {
                        
                        var item = context.Items.FirstOrDefault(i => i.ItemId == e.ItemId);
                        if (item == null)
                            throw new Exception("الصنف غير موجود");

                        if (item.Quantity < e.RemainingQty)
                            throw new Exception($"الكمية غير كافية للصنف {e.ItemName}");

                        
                        item.Quantity -= e.RemainingQty;

                        issuances.Add(new Issuance
                        {
                            IssuanceId = Guid.NewGuid(),
                            EmployeeId = yearIssuance.EmployeeId,
                            ItemId = e.ItemId,
                            Quantity = e.RemainingQty,
                            IssuanceDate = new DateTime(yearIssuance.Year, 1, 1),
                            CreatedDate = DateTime.Now,
                            Type = enIssuanceType.Entitled,
                            SignedReceiptPath = yearIssuance.SignedReceiptPath,
                            CreatedById = yearIssuance.CreatedById
                        });
                    }

                    context.Issuances.AddRange(issuances);
                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


    }
}
