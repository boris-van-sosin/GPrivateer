using System;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization;
using System.IO;

namespace FullBroadside.Importers
{
    public static class HierarchySerializer
    {
        static public string SerializeObject(TurretDefinition turretDef)
        {
            return SerializeObjectInner(turretDef, typeof(TurretDefinition));
        }

        static public string SerializeObject(ShipHullDefinition shipHullDef)
        {
            return SerializeObjectInner(shipHullDef, typeof(ShipHullDefinition));
        }

        static private string SerializeObjectInner(Object toSerialize, Type targetType)
        {
            DynamicTypeResolver tr = new DynamicTypeResolver();

            SerializerBuilder builder = new SerializerBuilder();
            builder.EnsureRoundtrip();
            builder.ConfigureDefaultValuesHandling(DefaultValuesHandling.Preserve);//.EmitDefaultsForValueTypes();
            builder.DisableAliases();
            //builder.WithTypeResolver(tr);
            ISerializer s = builder.Build();

            StringWriter tw = new StringWriter();
            s.Serialize(tw, toSerialize, targetType);
            return tw.ToString();
        }

        static public T LoadHierarchy<T>(TextReader reader)
        {
            Deserializer ds = new Deserializer();
            return ds.Deserialize<T>(reader);
        }
    }
}
