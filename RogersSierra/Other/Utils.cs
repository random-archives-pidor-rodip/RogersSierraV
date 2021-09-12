using System;
using System.Collections.Generic;
using System.Reflection;

namespace RogersSierra.Other
{
    /// <summary>
    /// Various help functions.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Random instance.
        /// </summary>
        public static readonly Random Random = new Random();

        /// <summary>
        /// Useful for cases when you have few bones like bone_left1, bone_right1. 
        /// This functinon allows to process them by just base bone name and bone count.
        /// </summary>
        /// <param name="baseBone">Shared part between all bones, for i.e: "veh_bone_".</param>
        /// <param name="totalBoneNumber">Total bone number (left + right), should be symmetrical.</param>
        /// <param name="action">Action to process.</param>
        public static void ProcessSideBones(string baseBone, int totalBoneNumber, Action<string> action)
        {
            for (int l = 0, r = 0; l + r < totalBoneNumber;)
            {
                // Generate bone name
                var bone = baseBone;

                if (l < 3)
                    bone += $"left_{l++ + 1}";
                else
                    bone += $"right_{r++ + 1}";

                action(bone);
            }
        }

        /// <summary>
        /// Useful for cases when you have few bones like bone_1, bone_2. 
        /// This functinon allows to process them by just base bone name and bone count.
        /// </summary>
        /// <param name="baseBone">Shared part between all bones, for i.e: "veh_bone_".</param>
        /// <param name="totalBoneNumber">Total bone number.</param>
        /// <param name="action">Action to process.</param>
        public static void ProcessMultipleBones(string baseBone, int totalBoneNumber, Action<string> action)
        {
            for (int i = 0; i < totalBoneNumber; i++)
            {
                // Generate bone name
                var bone = baseBone + (i +1);

                action(bone);
            }
        }

        /// <summary>
        /// Executes action for all fields of given type.
        /// </summary>
        /// <typeparam name="T">Type of field.</typeparam>
        /// <param name="obj">Class object.</param>
        /// <param name="action">Action to execute.</param>
        public static void ProcessAllClassFieldsByType<T>(object obj, Action<FieldInfo> action)
        {
            var fields = obj.GetType().GetFields();

            // Go through each field and find fields of given type
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (field.FieldType.BaseType == typeof(T) || field.FieldType == typeof(T))
                {
                    action(field);
                }
            }
        }

        /// <summary>
        /// Executes action for all field values of given type.
        /// </summary>
        /// <typeparam name="T">Type of field.</typeparam>
        /// <param name="instance">Class object.</param>
        /// <param name="action">Action to execute.</param>
        public static void ProcessAllValuesFieldsByType<T>(object instance, Action<T> action)
        {
            ProcessAllClassFieldsByType<T>(instance, field =>
            {
                var fieldValue = (T)field.GetValue(instance);

                action(fieldValue);
            });
        }

        /// <summary>
        /// Gets all field values of given type.
        /// </summary>
        /// <typeparam name="T">Field type.</typeparam>
        /// <param name="instance">Class instance to get values from.</param>
        /// <returns>Field values of given type.</returns>
        public static List<T> GetAllFieldValues<T>(object instance)
        {
            var fields = instance.GetType().GetFields();
            var list = new List<T>();
            
            // Go through every class field
            for(int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                // Check if it have type of T
                if(field.FieldType == typeof(T))
                    list.Add((T)field.GetValue(instance));
            }
            return list;
        }
    }
}
