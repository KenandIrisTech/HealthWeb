using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace CommonLib.Data
{
    public class CommonDB : IoTEntity
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(c => c.Ignore("IsDirty"));
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {

            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbexp)
            {
                foreach (DbEntityEntry entity in dbexp.Entries)
                {
                    string errorMessage = string.Format("was: {0}, is: {1}", entity.OriginalValues, entity.CurrentValues);
                    Console.WriteLine(errorMessage);
                }
                return 0;
            }
            catch (DbEntityValidationException validExp)
            {
                foreach (DbEntityValidationResult result in validExp.EntityValidationErrors)
                {
                    string errorMessage = result.ValidationErrors.Select(v => string.Format("{0}:{1}", v.PropertyName, v.ErrorMessage)).Aggregate(
                        (p, n) => p + "," + n);
                    Console.WriteLine(errorMessage);
                }
                return 0;
            }
            //foreach (var history in this.ChangeTracker.Entries()
            //  .Where(e => e.Entity is IModificationHistory && (e.State == EntityState.Added ||
            //          e.State == EntityState.Modified))
            //   .Select(e => e.Entity as IModificationHistory)
            //  )
            //{
            //    history.DateModified = DateTime.Now;
            //    if (history.DateCreated == DateTime.MinValue)
            //    {
            //        history.DateCreated = DateTime.Now;
            //    }
            //}
            //int result = base.SaveChanges();
            //foreach (var history in this.ChangeTracker.Entries()
            //                              .Where(e => e.Entity is IModificationHistory)
            //                              .Select(e => e.Entity as IModificationHistory)
            //  )
            //{
            //    history.IsDirty = false;
            //}
            //return result;
        }
    }
}
