﻿import { Link } from "./Link";

/**
 * Resource that provides navigation links.
 * @type T type of the resource.
 */
export class BrowsableResource<T>{

    /** Navigation links */
    public readonly links : Array<Link>
    /** Resource */
    public readonly resource : T
}