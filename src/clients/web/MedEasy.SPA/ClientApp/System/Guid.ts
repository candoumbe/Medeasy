﻿
export class Guid {
    /**
     * Builds a new Guid instance
     */
    private constructor(private guid : string){}

    /**
     * Generates a new Guid
     */
    public static newGuid(): Guid {
        let guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
            let r: number = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);

        });

        return new Guid(guid);
    }


    public static get empty() : Guid {
        return new Guid("00000000-0000-0000-0000-000000000000");
    }


    public toString() : string {
        return this.guid
    }
}