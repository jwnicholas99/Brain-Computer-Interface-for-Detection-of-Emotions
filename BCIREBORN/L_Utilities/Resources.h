#ifndef Resources_h
#define Resources_h

#include "Error.h"

#define MAX_RESOURCE_TYPES            64
#define MAX_RESOURCE_FIELDS_PER_TYPE  64

#define SYSTEM_ENVIRON_VARIABLE       "MERCURY"
#define MAIN_RESOURCE_DATABASE        "Data/Resources"
#define HOME_DIR_ENVIRON_VARIABLE     "HOME"
#define HOME_DIR_DATABASE             "MercuryResources"


// The Resources class definition.
typedef struct ResourceTypeStruct  ResourceType;
typedef struct ResourceEntryStruct ResourceEntry;

struct ResourceEntryStruct {

    char *name;
    char *value;

    int changed;

};

struct ResourceTypeStruct {

    char *name;
    int numFields;

    ResourceEntry entries[MAX_RESOURCE_FIELDS_PER_TYPE];
};


class Resources {

  public:

    Resources();
    ~Resources();

    PtErr Merge(char *szFileName);
    bool LoadText(char *szFileName);
    bool LoadBinary(char *szFileName);

    bool SaveText(char *szFileName);
    bool SaveBinary(char *szFileName);
    PtErr Get(char *type, char *field, int *value);
    PtErr Get(char *type, char *field, float *value);
    PtErr Get(char *type, char *field, char *value, int size);

    PtErr Put(char *type, char *field, int value);
    PtErr Put(char *type, char *field, float value);
    PtErr Put(char *type, char *field, char *value);

    PtErr Delete(char *type, char *field);

    PtErr HasChanged(char *type, int *changed);
    PtErr HasChanged(char *type, char *field, int *changed);

    PtErr Exists(char *type);

    PtErr Print();

  private:

    void  ClearFields();
    bool  ParseLine(char *szKey, char *szValue);

    ResourceType  *FindType(char *type);
    ResourceEntry *FindEntry(char *type, char *field);
    ResourceEntry *AddEntry(char *type, char *field);

    int fNumTypes;
    ResourceType fTypes[MAX_RESOURCE_TYPES];
    char mCurrentType[256];
};

/*--------------------------------------------------------------------------------

Public Functions

  Resources()
    Initialize resources from the main database of resources
    ($PLAINTALK/data/Resources).  Then merge any resources defined
    in the current user's home directory ($HOME/PlainTalkResources).

  ~Resources();
    Destructor.

  Merge(char *filename);
    Merge resources from the specified file.

  Write(char *filename);
    Write out resources to file.  Since this function is typically used for
    "probing" resources, it writes out the filename in a comment above each
    resource line indicating which resource file from which the entry originated.

  Get(char *type, char *field, char **value, int size);
    Get a value for a specified resource type and field name (ID or keyword)
    and place the result into the passed-in string.

  Get(char *type, char *field, int *value);
    Get a value for a specified resource type and field name (ID or keyword),
    interpret the result as an integer, and place itinto the passed-in integer.

  Get(char *type, char *field, float *value);
    Get a value for a specified resource type and field name (ID or keyword),
    interpret the result as a float, and place itinto the passed-in float.

  Put(char *type, char *field, char *value);
    Set a value for a specified resource type and field name (ID or keyword).

  Put(char *type, char *field, int value);
    Set a value for a specified resource type and field name (ID or keyword),
    where the original value is an integer.

  Put(char *type, char *field, float value);
    Set a value for a specified resource type and field name (ID or keyword),
    where the original value is a float.

  HasChanged(char *type, int *changed);
    Call this function to see if any fields within the specified
    resource type have been changed.  If so, then your source code
    should copy over the new values for these fields into the object,
    to ensure the parameter settings in the resources are consistent
    with your object.

    Unfortunately, this is needed since Resources are expected to be
    global to the process, and hence any module may be permitted to
    alter another module's resources!

  HasChanged(char *type, char *field, int *changed);
    Same as previous function, except we check to see if a specific
    resource entry has changed.

--------------------------------------------------------------------------------*/

#endif /* _Resources_h_ */

