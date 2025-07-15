using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon
{
    public enum Error
    {
        OK = 0,
        NOT_FOUND = -1,
        GENERIC_ERROR = -2,
        COMMAND_NOT_FOUND = -3,
        M2F_BAD_DATA = -4,
        F2M_BAD_DATA = -5,
        MYSQL_CONNECTION_ERROR = -6,
        MYSQL_NOT_CONNECTED = -7,
        MYSQL_NON_QUERY_ERROR = -8,
        MYSQL_BAD_INSERT = -9,
        MYSQL_PRIMARY_KEY_MISSING = -10,
        MYSQL_READ_ERROR =-11,
        RECIPE_NOT_FOUND_OR_BAD = -12,
        RSWARE_NOT_OPENED = -13,
        ROBOT_IS_NOT_READY = -14,
        USER_NOT_CORRECT = -15,
        MYSQL_FIELD_KEY_MISSING = -16,
    }

}
