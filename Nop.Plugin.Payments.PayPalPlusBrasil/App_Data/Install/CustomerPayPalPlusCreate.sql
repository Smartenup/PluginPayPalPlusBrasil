CREATE TABLE [dbo].[Customer_PayPalPlus] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[CustomerId] int not null,
	[RememberedCards] nvarchar(max) null
)


CREATE TABLE [dbo].[Order_Note_PayPalPlus] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[OrderId] int not null,
	[ControlNoteStatusId] int not null
)


insert ScheduleTask  (name, Seconds, Type, Enabled, StopOnError)values ( 'PayPalPlusPrevisaoEnvioTask', 600, 'Nop.Plugin.Payments.PayPalPlusBrasil.PayPalPlusPrevisaoEnvioTask, Nop.Plugin.Payments.PayPalPlusBrasil',0,0)