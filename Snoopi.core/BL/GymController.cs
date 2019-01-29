using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using AnyGym.core.DAL;
using System.Text.RegularExpressions;
using dg.Sql.Connector;
using dg.Utilities;
using System.Collections;

namespace AnyGym.core.BL
{
    public static class GymController
    {
        public static void UpdateImagesCache(Int32 GymId)
        {
            GymImageCollection collImages = GymImageCollection.Where(GymImage.Columns.GymId, GymId);
            collImages.Sort(@"SortOrder");

            StringBuilder sb = new StringBuilder();
            foreach (GymImage image in collImages)
            {
                if (sb.Length > 0) sb.Append(@"|");
                sb.Append(image.FileName);
            }

            Query.New<Gym>()
                .Update(Gym.Columns.ImagesCache, sb.ToString())
                .Where(Gym.Columns.GymId, GymId)
                .Execute();
        }
        public static void UpdateBusinessHoursCache(Int32 GymId)
        {
            GymBusinessHourCollection collBizHours = GymBusinessHourCollection.Where(GymBusinessHour.Columns.GymId, GymId);

            StringBuilder sb = new StringBuilder();
            foreach (GymBusinessHour hour in collBizHours)
            {
                if (sb.Length > 0) sb.Append(@",");
                sb.Append((int)hour.Day);
                sb.Append(',');
                if (hour.StartHour >= 0)
                {
                    sb.Append('n');
                    sb.Append(hour.StartHour.ToString().PadLeft(2, '0'));
                    sb.Append(':');
                    sb.Append(hour.StartMinute.ToString().PadLeft(2, '0'));
                    sb.Append('-');
                    sb.Append(hour.EndHour.ToString().PadLeft(2, '0'));
                    sb.Append(':');
                    sb.Append(hour.EndMinute.ToString().PadLeft(2, '0'));
                }
                if (hour.PeakStartHour >= 0)
                {
                    sb.Append('p');
                    sb.Append(hour.PeakStartHour.ToString().PadLeft(2, '0'));
                    sb.Append(':');
                    sb.Append(hour.PeakStartMinute.ToString().PadLeft(2, '0'));
                    sb.Append('-');
                    sb.Append(hour.PeakEndHour.ToString().PadLeft(2, '0'));
                    sb.Append(':');
                    sb.Append(hour.PeakEndMinute.ToString().PadLeft(2, '0'));
                }
            }

            Query.New<Gym>()
                .Update(Gym.Columns.BizHoursCache, sb.ToString())
                .Where(Gym.Columns.GymId, GymId)
                .Execute();
        }
    }
}
