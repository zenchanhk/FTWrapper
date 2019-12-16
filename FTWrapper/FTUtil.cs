using Futu.OpenApi.Pb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTWrapper
{
    public class FTUtil
    {
        public static QotCommon.KLType SubTypeToKLType(QotCommon.SubType subType)
        {
            QotCommon.KLType result = QotCommon.KLType.KLType_Unknown;
            switch (subType)
            {
                case QotCommon.SubType.SubType_KL_1Min:
                    result = QotCommon.KLType.KLType_1Min;
                    break;
                case QotCommon.SubType.SubType_KL_5Min:
                    result = QotCommon.KLType.KLType_5Min;
                    break;
                case QotCommon.SubType.SubType_KL_15Min:
                    result = QotCommon.KLType.KLType_15Min;
                    break;
                case QotCommon.SubType.SubType_KL_30Min:
                    result = QotCommon.KLType.KLType_30Min;
                    break;
                case QotCommon.SubType.SubType_KL_60Min:
                    result = QotCommon.KLType.KLType_60Min;
                    break;
            }
            return result;
        }

        public static QotCommon.SubType KLTypeToSubType(QotCommon.KLType klType)
        {
            QotCommon.SubType result = QotCommon.SubType.SubType_None;
            switch (klType)
            {
                case QotCommon.KLType.KLType_1Min:
                    result = QotCommon.SubType.SubType_KL_1Min;
                    break;
                case QotCommon.KLType.KLType_5Min:
                    result = QotCommon.SubType.SubType_KL_5Min;
                    break;
                case QotCommon.KLType.KLType_15Min:
                    result = QotCommon.SubType.SubType_KL_15Min;
                    break;
                case QotCommon.KLType.KLType_30Min:
                    result = QotCommon.SubType.SubType_KL_30Min;
                    break;
                case QotCommon.KLType.KLType_60Min:
                    result = QotCommon.SubType.SubType_KL_60Min;
                    break;
            }
            return result;
        }

        public static QotCommon.SubType IntToSubType(int interval)
        {
            QotCommon.SubType result = QotCommon.SubType.SubType_None;
            switch (interval)
            {
                case 1:
                    result = QotCommon.SubType.SubType_KL_1Min;
                    break;
                case 5:
                    result = QotCommon.SubType.SubType_KL_5Min;
                    break;
                case 15:
                    result = QotCommon.SubType.SubType_KL_15Min;
                    break;
                case 30:
                    result = QotCommon.SubType.SubType_KL_30Min;
                    break;
                case 60:
                    result = QotCommon.SubType.SubType_KL_60Min;
                    break;
            }
            return result;
        }

        public static QotCommon.KLType IntToKLType(int interval)
        {
            QotCommon.KLType result = QotCommon.KLType.KLType_Unknown;
            switch (interval)
            {
                case 1:
                    result = QotCommon.KLType.KLType_1Min;
                    break;
                case 5:
                    result = QotCommon.KLType.KLType_5Min;
                    break;
                case 15:
                    result = QotCommon.KLType.KLType_15Min;
                    break;
                case 30:
                    result = QotCommon.KLType.KLType_30Min;
                    break;
                case 60:
                    result = QotCommon.KLType.KLType_60Min;
                    break;
            }
            return result;
        }
    }
}
