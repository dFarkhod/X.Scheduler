import { ErrorHandler } from "@angular/core";

export class AppErrorHandler implements ErrorHandler {

    handleError(error: any): void {
        //TODO: Show error properly like notification and send it to logger service
        alert('Unexpected error occured. ' + error);
        console.log(error);
    }
    
}