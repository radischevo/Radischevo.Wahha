using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Data.Provider;
using Radischevo.Wahha.Web.Caching;

public enum MessageDirection : int
{
    Outgoing = 0,
    Incoming = 1
}

public class MessageInfo
{
    public DateTime Date { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Message { get; set; }
    public MessageDirection Direction { get; set; }
}

public class SmsMaterializer : IDbMaterializer<MessageInfo>
{
    #region IDbMaterializer<MessageInfo> Members
    public MessageInfo Materialize(IValueSet source)
    {
        return Materialize(new MessageInfo(), source);
    }

    public MessageInfo Materialize(MessageInfo entity, IValueSet source)
    {
        entity.Date = source.GetValue<DateTime>("Date");
        entity.From = source.GetValue<string>("From");
        entity.To = source.GetValue<string>("To");
        entity.Message = source.GetValue<string>("Message");
        entity.Direction = (MessageDirection)source.GetValue<int>("Direction");

        return entity;
    }
    #endregion
}

public class SmsRepository : DbRepository<MessageInfo>
{
    public DbDataProvider DataProvider { get; private set; }
    public IDbMaterializer<MessageInfo> Materializer { get; private set; }

    public SmsRepository()
        : base("sms")
    {
        DataProvider = DbDataProvider.Create<SqlDbDataProvider>(
            @"Data Source=euro\sqlexpress;Initial Catalog=outlook;User ID=sa;Password=Trfnthbyf_Lelpbycrfz");
        Cache = new AspNetCacheProvider();
        ExpirationTimeout = TimeSpan.FromMinutes(30);
        Materializer = new SmsMaterializer();
    }

    public IEnumerable<MessageInfo> Select(int page)
    {
        DbCommandDescriptor descriptor = new DbCommandDescriptor(
            @"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY [Date] ASC) AS [Row], [Date], [From], [To], [Message], [Direction] 
                FROM [dbo].[message_Final]) AS M WHERE M.[Row] BETWEEN {0} AND {1}", 
            CommandType.Text, new object[] { (page - 1) * 50 + 1, page * 50 });

        return Select(descriptor);
    }

    protected override IEnumerable<MessageInfo> ExecuteSelect(DbCommandDescriptor command)
    {
        return DataProvider.Execute(command).AsDataReader().Select(v => Materializer.Materialize(v)).ToList();
    }

    protected override MessageInfo ExecuteSingle(DbCommandDescriptor command)
    {
        IValueSet values = DataProvider.Execute(command).AsDataReader().FirstOrDefault();
        return (values == null) ? null : Materializer.Materialize(values);
    }

    protected override MessageInfo ExecuteSave(MessageInfo entity)
    {
        throw new NotImplementedException();
    }

    protected override MessageInfo ExecuteDelete(MessageInfo entity)
    {
        throw new NotImplementedException();
    }
}
