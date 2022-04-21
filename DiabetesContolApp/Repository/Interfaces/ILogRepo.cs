using System.Collections.Generic;
using System.Threading.Tasks;
using DiabetesContolApp.Models;

namespace DiabetesContolApp.Repository.Interfaces
{
    public interface ILogRepo
    {
        /// <summary>
        /// Deletes the log with the given ID from the database.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>Returns true if it was deleted, if an error occurs then false</returns>
        Task<bool> DeleteLogAsync(int logID);

        /// <summary>
        /// Gets all LogModelDAOs then converts them into LogModels.
        /// </summary>
        /// <returns>List of LogModels, might be empty.</returns>
        Task<List<LogModel>> GetAllLogsAsync();

        /// <summary>
        /// Gets all LogModelDAOs with a given
        /// DayProfile ID, converts them into LogModels.
        /// </summary>
        /// <param name="dayProfileID"></param>
        /// <returns>List of LogModels who has the given DayProfile ID.</returns>
        Task<List<LogModel>> GetAllLogsWithDayProfileIDAsync(int dayProfileID);

        /// <summary>
        /// Gets all LogModelDAOs with the given reminderID.
        /// Then converts them into LogModels.
        /// </summary>
        /// <param name="reminderID"></param>
        /// <returns>List of LogModels with the given reminder ID, might be empty.</returns>
        Task<List<LogModel>> GetAllLogsWithReminderIDAsync(int reminderID);

        /// <summary>
        /// Gets the logDAO with the given logID, if it exists,
        /// then convert it to a logModel.
        /// </summary>
        /// <param name="logID"></param>
        /// <returns>
        /// Return the LogModel with the given logID or
        /// null if no log with the given ID was found.
        /// </returns>
        Task<LogModel> GetLogAsync(int logID);

        /// <summary>
        /// Converts LogModel to DAO object. Inserts the DAO
        /// into the database.
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns>Returns true if inserted else false.</returns>
        Task<bool> InsertLogAsync(LogModel newLog);

        /// <summary>
        /// Updates the log with the matching ID.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>True if it was updated, else false.</returns>
        Task<bool> UpdateLogAsync(LogModel log);
    }
}