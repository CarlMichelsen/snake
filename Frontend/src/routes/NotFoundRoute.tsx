import { type FC } from "react";
import {Link} from "react-router";

const NotFoundRoute: FC = () => {
    return <>
        <p className="text-center mt-96 text-lg">Page Not Found</p>
        <Link to="/" className="text-center block">Home</Link>
    </>
}

export default NotFoundRoute;