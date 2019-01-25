#include "Utilities.h"
static char *kResourceMagic = "<Resources>v1.0";
#define DELETED_ENTRY_VALUE "*DELETED*"

enum {
    errCannotOpenFile = -1,
    errEntryAlreadyExists = -2,
    errEnvVarNotFound = -3,
    errGetFailed = -4,
    errHomeEnvNotSet = -5,
    errMergeFailed = -6,
    errNoValueForKeyword = -7,
    errOutOfEntries = -8,
    errOutOfTypes = -9,
    errPeriodNotFound = -10,
    errPutFailed = -11,
    errResourceNotFound = -12,
    errStringsOverlap = -13,
    errSysEnvNotSet = -14,
    errTypeNotFound = -15,
    errTypeNotYetSet = -16
};


Resources::Resources()
{
    /* Make sure to clear the fields!  ***PSI***  30-Jan-96 */
    ClearFields();
}

Resources::~Resources()
{
    for (int t=0; t<fNumTypes; t++) {

	for (int f=0; f<fTypes[t].numFields; f++) {

	    if (fTypes[t].entries[f].name)
	      free(fTypes[t].entries[f].name);

	    if (fTypes[t].entries[f].value)
	      free(fTypes[t].entries[f].value);

	}

	if (fTypes[t].name)
	  free(fTypes[t].name);
    }
}

/* Initialize the fields to zero.  ***PSI***  30-Jan-96 */
void
 Resources::ClearFields()
{
    memset(this, 0, sizeof(*this));
}

ResourceType *
  Resources::FindType(char *type)
{
    for (int t=0; t<fNumTypes; t++)
      if (!strcmp(type, fTypes[t].name))
	// Found.
	return &(fTypes[t]);

    // Not found.
    return 0;
}

ResourceEntry *
  Resources::FindEntry(char *typeName, char *field)
{
    ResourceType *type;

    if (type = FindType(typeName))
      for (int f=0; f<type->numFields; f++)
	if (!strcmp(field, type->entries[f].name))
	  // Do not return deleted entries.
	  if (strcmp(type->entries[f].value, DELETED_ENTRY_VALUE))
	    // Found.
	    return &(type->entries[f]);

    // Not found.
    return 0;
}

ResourceEntry *
  Resources::AddEntry(char *typeName, char *field)
{
    // First, see if the entry exists, so that we
    // do not accidentally add the same entry more
    // than once.
    if (FindEntry(typeName, field)) {
	Error("Resources::AddEntry", -1, "errEntryAlreadyExists");
	return 0;
    }

    // Next, see if the type exists.  If so, add this
    // entry to the found type.  Otherwise, create a new
    // type, and make this the first entry for it.
    ResourceType *type = FindType(typeName);
    ResourceEntry *entry = 0;

    if (type) {

	// This resource type was found, so return the next
	// available entry for this type.

	if (type->numFields >= MAX_RESOURCE_FIELDS_PER_TYPE) {
	    Error("Resources::AddEntry", -1, "errOutOfEntries");
	    return 0;
	}

	entry = &(type->entries[type->numFields++]);

	if (field && strlen(field))
	  entry->name = _strdup(field);
	else
	  entry->name = 0;

    } else {

	// This resource type was not found, so use the next 
	// available type, and return the first entry in it.

	if (fNumTypes >= MAX_RESOURCE_TYPES - 1) {
	    Error("Resources::AddEntry", -1, "errOutOfTypes");
	    return 0;
	}

	type = &(fTypes[fNumTypes++]);

	if (typeName && strlen(typeName))
	  type->name = _strdup(typeName);
	else
	  type->name = 0;

	type->numFields = 1;

	entry = &(type->entries[0]);

	if (field && strlen(field))
	  entry->name = _strdup(field);
	else
	  entry->name = 0;

    }

    return entry;
}

PtErr
  Resources::Delete(char *type, char *field)
{
    ResourceEntry *entry;

    if (!(entry = FindEntry(type, field)))
      // The entry did not even exist!
      return errOkay;

    // Mark this entry as deleted.
    return Put(type, field, DELETED_ENTRY_VALUE);
}

PtErr
  Resources::Put(char *type, char *field, char *value)
{
    ResourceEntry *entry;

    if (!(entry = FindEntry(type, field)))
      if (!(entry = AddEntry(type, field)))
	return Error("Resources::Put", -1, "errOutOfEntries");

    // Free previous values before overwriting.
    if (entry->value)
      free(entry->value);

    entry->value = _strdup(value);

    // Flag this entry as having been changed (or is new).
    entry->changed = 1;

    return errOkay;
}

PtErr Resources::Put(char *type, char *field, int value)
{
    char buffer[1024];

	sprintf_s(buffer, "%d", value);
    return Put(type, field, buffer);
}

PtErr  Resources::Put(char *type, char *field, float value)
{
    char buffer[1024];

	sprintf_s(buffer, "%f", value);
    return Put(type, field, buffer);
}

PtErr
  Resources::Get(char *type, char *field, char *value, int size)
{
    ResourceEntry *entry;

    if (!(entry = FindEntry(type, field)))
      return -1; // Error("Resources::Get", -1, "errResourceNotFound");

	strncpy_s(value, size, entry->value, size);

    return errOkay;
}

PtErr
  Resources::Get(char *type, char *field, int *value)
{
    char buffer[1024];

    if (Get(type, field, buffer, sizeof(buffer)) != errOkay)
      return -1; // Error("Resources::Get", -1, "errGetFailed");

    if (!strcmp(buffer, "TRUE"))
      *value = 1;
    else if (!strcmp(buffer, "FALSE"))
      *value = 0;
    else
      *value = (int) atoi(buffer);

    return errOkay;
}

PtErr
  Resources::Get(char *type, char *field, float *value)
{
    char buffer[1024];

    if (Get(type, field, buffer, sizeof(buffer)) != errOkay)
      return -1; // Error("Resources::Get", -1, "errGetFailed");

    *value = (float) atof(buffer);

    return errOkay;
}

PtErr Resources::Merge(char *szFileName)
{
    if (LoadBinary(szFileName))
    {
        return errOkay;
    }
    else if (LoadText(szFileName))
    {
        return errOkay;
    }
    else
    {
        return Error("Resources::Merge", -1, "errFileLoad");
    }
}

bool Resources::LoadText(char *szFileName)
{
    File mFile;
    char *keyword, *value, szLine[kMaxLine];
    if (!mFile.Open(szFileName, "rt"))
    {
        return false;
    }
    mCurrentType[0] = 0;
    while (!mFile.IsEOF())
    {
        if (mFile.GetLine(szLine, sizeof(szLine)))
        {
            // Eliminate any comments within the line.
            for (char *b = szLine; *b; b++) 
            {
	            if ((*b == '#') || (*b == ';')) 
                {
		            *b = 0;
                    break;
                }
            }

			char *next_token = NULL;

	        // Try to get the keyword (if any).
			if (!(keyword = strtok_s(szLine, WHITESPACE, &next_token)))
            {
	            // This was a blank line.
	            continue;
            }

	        // Try to get the value (required).
			if (!(value = strtok_s(0, "\n", &next_token))) // ccwang change WHITESPACE to NEWLINE
            {
                Error("Resources::Merge", -1, "errNoValueForKeyword");
                continue;
            }
            ParseLine(keyword, value);
        }
    }
    mFile.Close();
    return true;
}

bool Resources::LoadBinary(char *szFileName)
{
    File mFile;
    char szKey[kMaxLine], szValue[kMaxLine];
    if (!mFile.Open(szFileName, "rb", kResourceMagic, 0x0f))
    {
        return false;
    }
    mCurrentType[0] = 0;
    while (!mFile.IsEOF())
    {
        if (mFile.ReadString(szKey, sizeof(szKey)) && mFile.ReadString(szValue, sizeof(szValue)))
        {
            ParseLine(szKey, szValue);
        }
    }
    mFile.Close();
    return true;
}

bool Resources::ParseLine(char *keyword, char *value)
{
    if (!strcmp(keyword, "Resource")) 
    {

	    // This line names a new resource type, which means that
	    // the fields to follow correspond to this resource type.
	    strcpy_s(mCurrentType, value);

	} 
    else 
    {
	    // Try to extract the type from a "compound" keyword.
	    // That is, a keyword of the form "<Type>.<Keyword>",
	    // which is useful when inputting resources from a
	    // cut-and-paste from a resource printout (such as
	    // for running under the debugger).  The extracted
	    // resource type overrides the current type for the
	    // current line only.  If the next line has no type
	    // embedded in a "compound" keyword, the previous
	    // current resource type is used.  That is, we do
	    // not switch the current type to the extracted type.

	    char *period = strchr(keyword, '.');

	    if (period) 
        {

		    // Destructively subdivide the keyword string
		    // into two new strings.

		    *period = 0;

		    // The original keyword now points to the new
		    // (temporary) resource type, and the new
		    // keyword is located just after the period.

		    char *tempType    = keyword;
		    char *tempKeyword = period + 1;

		    if (Put(tempType, tempKeyword, value) != errOkay)
            {
		        Error("Resources::Merge", -1, "errPutFailed");
                return false;
            }
	    } 
        else 
        {
		    // Check to make sure the user has already set the type.
		    if (mCurrentType[0] == 0)
            {
                Error("Resources::Merge", -1, "errTypeNotYetSet");
                return false;
            }

		    // Associate this value into a resource field.
		    if (Put(mCurrentType, keyword, value) != errOkay)
            {
                Error("Resources::Merge", -1, "errPutFailed");
                return false;
            }      
	    }
	}
    return true;
}

bool Resources::SaveText(char *szFileName)
{
    File mFile;
    if (!mFile.Open(szFileName, "wt"))
    {
        return false;
    }

    for (int t=0; t<fNumTypes; t++) 
    {
        mFile.WriteString("Resource \t");
        mFile.WriteString(fTypes[t].name);
        mFile.PutLine();         // Append a new line for readble
	    for (int f=0; f<fTypes[t].numFields; f++) 
        {
            mFile.WriteString(fTypes[t].entries[f].name);
			mFile.WriteString("\t");
            mFile.WriteString(fTypes[t].entries[f].value);
            mFile.PutLine();
        }
        mFile.PutLine();
	}
    mFile.Close();
    return true;
}

bool Resources::SaveBinary(char *szFileName)
{
    File mFile;
    if (!mFile.Open(szFileName, "wb", kResourceMagic, 0x0f))
    {
        return false;
    }

    for (int t=0; t<fNumTypes; t++) 
    {
        mFile.WriteString("Resource");
        mFile.WriteString(fTypes[t].name);
	    for (int f=0; f<fTypes[t].numFields; f++) 
        {
            mFile.WriteString(fTypes[t].entries[f].name);
            mFile.WriteString(fTypes[t].entries[f].value);
        }
	}
    mFile.Close();
    return true;
}



PtErr
  Resources::HasChanged(char *typeName, int *changed)
{
    ResourceType *type;

    if (!(type = FindType(typeName)))
      return Error("Resources::HasChanged", -1, "errTypeNotFound");

    // If any of the entries within a resource type have changed,
    // then this resource type is considered also to have changed.

    for (int f=0; f<type->numFields; f++)
      if (type->entries[f].changed) {
	  *changed = 1;
	  return errOkay;
      }

    *changed = 0;
    return errOkay;
}

PtErr
  Resources::HasChanged(char *type, char *field, int *changed)
{
    ResourceEntry *entry;

    if (!(entry = FindEntry(type, field)))
      return Error("Resources::HasChanged", -1, "errResourceNotFound");

    if (entry->changed)
      *changed = 1;
    else
      *changed = 0;

    return errOkay;
}

PtErr
  Resources::Print ()
{
    for (int t=0; t<fNumTypes; t++) {

	for (int f=0; f<fTypes[t].numFields; f++) {

	    char buffer[256];

		strcpy_s(buffer, fTypes[t].name);
		strcat_s(buffer, ".");
		strcat_s(buffer, fTypes[t].entries[f].name);
	    PrintMsg ("%-32s %s\n", buffer, fTypes[t].entries[f].value);
	}
    }
	
    return errOkay;
}

// NOTE: This returns "errOkay" (usually zero) when the resource type *DOES exist.
// This is backwards if one tests the return value as a boolean instead of an error code!
PtErr
  Resources::Exists(char *type)
{
    if (FindType(type))
      // Exists.
      return errOkay;
    else
      // Does *NOT* exist.
      return (-1);
}
