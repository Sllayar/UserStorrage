using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserStorrage6.Model.DB
{
    public enum Status
    {
        Enable,
        Disable,
        Delete
    }

    public enum Type
    {
        /// <summary>
        /// персональный (связь с пользователем)
        /// </summary>
        Personal,
        /// <summary>
        /// системный (УЗ, необходимая для работы системы (подключения, интеграции))
        /// </summary>
        System,
        /// <summary>
        ///  техническая (админские УЗ для
        ///обслуживания ИС, доступ к БД, ВМ и тд.)
        /// </summary>
        Tehnical
    }
}
