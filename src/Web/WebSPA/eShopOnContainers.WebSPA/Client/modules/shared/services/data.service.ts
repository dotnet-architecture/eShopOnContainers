import { Injectable } from '@angular/core';
import { Http, Response, RequestOptionsArgs, RequestMethod, Headers } from '@angular/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class DataService {

    constructor(public http: Http) { }

    get(url: string, params?: any) {
        let options: RequestOptionsArgs = {};
        options.headers = new Headers();
        this.addCors(options);
        
        return this.http.get(url, options).map(((res: Response) => {
            return res.json();
        })).catch(this.handleError);
    }

    post(url: string, data: any, params?: any) {
        return this.http.post(url, data, params);
    }

    private addCors(options: RequestOptionsArgs): RequestOptionsArgs {
        options.headers.append('Access-Control-Allow-Origin', '*');
        return options;
    }

    private handleError(error: any) {
        console.error('server error:', error);
        if (error instanceof Response) {
            let errMessage = '';
            try {
                errMessage = error.json().error;
            } catch (err) {
                errMessage = error.statusText;
            }
            return Observable.throw(errMessage);
        }
        return Observable.throw(error || 'Node.js server error');
    }
}
