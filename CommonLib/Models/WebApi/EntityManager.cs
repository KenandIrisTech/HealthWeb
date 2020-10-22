using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace CommonLib.Models.WebApi
{
    public class EntityManager
    {
        private object t;
        public EntityManager(object t)
        {
            this.t = t;
            this.CollectionProps = t.GetType().GetProperties().Where(m => m.PropertyType.IsGenericType && m.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)).ToArray();
            this.EntityProps = t.GetType().GetProperties().Where(m => (!m.PropertyType.IsGenericType || m.PropertyType == typeof(Nullable<int>)) && !"CREATED_TIME.UPDATED_TIME.STATE.".Contains(m.Name + ".")).ToArray();
            this.UpdateTimeProps = t.GetType().GetProperties().Where(m => "CREATED_TIME.UPDATED_TIME.".Contains(m.Name + ".")).ToArray();
            this.UpdateIdProps = t.GetType().GetProperties().Where(m => "CREATED_USER_ID.UPDATED_USER_ID.".Contains(m.Name + ".")).ToArray();
            this.StateProps = t.GetType().GetProperties().Where(m => "STATE.".Contains(m.Name + ".")).ToArray();
            this.MetadataTypeAttr = t.GetType().GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            this.ModelMetaData = (this.MetadataTypeAttr != null)
                ? ModelMetadataProviders.Current.GetMetadataForType(null, this.MetadataTypeAttr.MetadataClassType)
                : ModelMetadataProviders.Current.GetMetadataForType(null, t.GetType());
        }

        public PropertyInfo[] CollectionProps { get; set; }
        public PropertyInfo[] EntityProps { get; set; }
        public PropertyInfo[] UpdateTimeProps { get; set; }
        public PropertyInfo[] UpdateIdProps { get; set; }

        public PropertyInfo[] StateProps { get; set; }

        public MetadataTypeAttribute MetadataTypeAttr { get; set; }
        public ModelMetadata ModelMetaData { get; set; }

    }
}
