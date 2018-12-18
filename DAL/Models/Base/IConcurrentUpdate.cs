using System;

namespace Fotron.DAL.Models.Base
{
    public interface IConcurrentUpdate
    {

        void OnConcurrencyStampRegen();
    }

    public static class ConcurrentStamp
    {

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
