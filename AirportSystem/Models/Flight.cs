using System;
using System.ComponentModel.DataAnnotations;

namespace AirportSystem.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Flight number is required.")]
        [StringLength(10, ErrorMessage = "Flight number cannot exceed 10 characters.")]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "Airline name is required.")]
        [StringLength(50, ErrorMessage = "Airline name cannot exceed 50 characters.")]
        public string Airline { get; set; }

        [Required(ErrorMessage = "Departure time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required.")]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(Flight), nameof(ValidateFlightTimes))]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Departure airport is required.")]
        public int DepartureAirportId { get; set; }

        [Required(ErrorMessage = "Arrival airport is required.")]
        [CustomValidation(typeof(Flight), nameof(ValidateDifferentAirports))]
        public int ArrivalAirportId { get; set; }

        public Airport DepartureAirport { get; set; }
        public Airport ArrivalAirport { get; set; }

        // Custom validation for flight times
        public static ValidationResult ValidateFlightTimes(DateTime arrivalTime, ValidationContext context)
        {
            var instance = (Flight)context.ObjectInstance;
            if (arrivalTime <= instance.DepartureTime)
            {
                return new ValidationResult("Arrival time must be after departure time.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateDifferentAirports(int arrivalAirportId, ValidationContext context)
        {
            var instance = (Flight)context.ObjectInstance;
            if (arrivalAirportId == instance.DepartureAirportId)
            {
                return new ValidationResult("Arrival airport must be different from departure airport.");
            }
            return ValidationResult.Success;
        }
    }
}
