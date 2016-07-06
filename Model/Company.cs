using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Компания
    /// </summary>
    public class Company
    {
        /// <summary>
        /// Название компании
        /// </summary>
        public string NameCompany { get; set; }
        /// <summary>
        /// Логотип компании
        /// </summary>
        public byte[] LogoCompany { get; set; }
        /// <summary>
        /// Дата создания компании
        /// </summary>
        public string YearOfFoundation { get; set; }
        /// <summary>
        /// Расположение компании
        /// </summary>
        public LocationCompany Location { get; set; }
        /// <summary>
        /// Ключевые сотрудники
        /// </summary>
        // public List<People> KeyPeople { get; set; }
        public string KeyPeople { get; set; }
        /// <summary>
        /// Уставной капитал
        /// </summary>
        public Capital CharterCapital { get; set; }
    }

    /// <summary>
    /// Расположение компании
    /// </summary>
    public class LocationCompany
    {
        /// <summary>
        /// Страна
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
    }

    /// <summary>
    /// Сотрудник
    /// </summary>
    public class People
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Занимаемая должность
        /// </summary>
        public string Position { get; set; }
    }
    /// <summary>
    /// Уставной капитал
    /// </summary>
    public class Capital
    {
        /// <summary>
        /// Размер капитала
        /// </summary>
        public string CharterCapital { get; set; }
        /// <summary>
        /// дата(По состоянию на)
        /// </summary>
        public string Date { get; set; }
    }
}
