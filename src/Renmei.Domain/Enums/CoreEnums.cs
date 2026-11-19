namespace Renmei.Domain.Enums;

public enum UserStatus { Active, Locked, Deleted }
public enum KanjiStatus { Draft, InReview, Published, Archived }
public enum ReadingType { On, Kun }
public enum MeaningType { HanViet, Meaning }
public enum TokenPurpose { VerifyEmail, ResetPassword }
public enum ImportBatchStatus { Running, Completed, Failed, Partial }