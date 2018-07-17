import { Injectable }                   from '@angular/core';
import { Http, Response, Headers }      from '@angular/http';
import 'rxjs/add/operator/map';
import { Observable }                   from 'rxjs/Observable';
import { Subject }                      from 'rxjs/Subject';
import { Router }                       from '@angular/router';
import { ActivatedRoute }               from '@angular/router';
import { ConfigurationService }         from './configuration.service';
import { StorageService }               from './storage.service';

@Injectable()
export class SecurityService {

    private actionUrl: string;
    private headers: Headers;
    private storage: StorageService;
    private authenticationSource = new Subject<boolean>();
    authenticationChallenge$ = this.authenticationSource.asObservable();
    private authorityUrl = '';

    constructor(private _http: Http, private _router: Router, private route: ActivatedRoute, private _configurationService: ConfigurationService, private _storageService: StorageService) {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
        this.storage = _storageService;

        this._configurationService.settingsLoaded$.subscribe(x => {
            this.authorityUrl = this._configurationService.serverSettings.identityUrl
            this.storage.store('IdentityUrl', this.authorityUrl);
        });

        if (this.storage.retrieve('IsAuthorized') !== '') {
            this.IsAuthorized = this.storage.retrieve('IsAuthorized');
            this.authenticationSource.next(true);
            this.UserData = this.storage.retrieve('userData');
        }
    }

    public IsAuthorized: boolean;

    public GetToken(): any {
        return this.storage.retrieve('authorizationData');
    }

    public ResetAuthorizationData() {
        this.storage.store('authorizationData', '');
        this.storage.store('authorizationDataIdToken', '');

        this.IsAuthorized = false;
        this.storage.store('IsAuthorized', false);
    }

    public UserData: any;
    public SetAuthorizationData(token: any, id_token: any) {
        if (this.storage.retrieve('authorizationData') !== '') {
            this.storage.store('authorizationData', '');
        }

        this.storage.store('authorizationData', token);
        this.storage.store('authorizationDataIdToken', id_token);
        this.IsAuthorized = true;
        this.storage.store('IsAuthorized', true);

        this.getUserData()
            .subscribe(data => {
                this.UserData = data;
                this.storage.store('userData', data);
                // emit observable
                this.authenticationSource.next(true);
                window.location.href = location.origin;
            },
            error => this.HandleError(error),
            () => {
                console.log(this.UserData);
            });
    }

    public Authorize() {
        this.ResetAuthorizationData();

        let authorizationUrl = this.authorityUrl + '/connect/authorize';
        let client_id = 'js';
        let redirect_uri = location.origin + '/';
        let response_type = 'id_token token';
        let scope = 'openid profile orders basket marketing locations webshoppingagg orders.signalrhub';
        let nonce = 'N' + Math.random() + '' + Date.now();
        let state = Date.now() + '' + Math.random();

        this.storage.store('authStateControl', state);
        this.storage.store('authNonce', nonce);

        let url =
            authorizationUrl + '?' +
            'response_type=' + encodeURI(response_type) + '&' +
            'client_id=' + encodeURI(client_id) + '&' +
            'redirect_uri=' + encodeURI(redirect_uri) + '&' +
            'scope=' + encodeURI(scope) + '&' +
            'nonce=' + encodeURI(nonce) + '&' +
            'state=' + encodeURI(state);

        window.location.href = url;
    }

    public AuthorizedCallback() {
        this.ResetAuthorizationData();

        let hash = window.location.hash.substr(1);

        let result: any = hash.split('&').reduce(function (result: any, item: string) {
            let parts = item.split('=');
            result[parts[0]] = parts[1];
            return result;
        }, {});

        console.log(result);

        let token = '';
        let id_token = '';
        let authResponseIsValid = false;

        if (!result.error) {

            if (result.state !== this.storage.retrieve('authStateControl')) {
                console.log('AuthorizedCallback incorrect state');
            } else {

                token = result.access_token;
                id_token = result.id_token;

                let dataIdToken: any = this.getDataFromToken(id_token);
                console.log(dataIdToken);

                // validate nonce
                if (dataIdToken.nonce !== this.storage.retrieve('authNonce')) {
                    console.log('AuthorizedCallback incorrect nonce');
                } else {
                    this.storage.store('authNonce', '');
                    this.storage.store('authStateControl', '');

                    authResponseIsValid = true;
                    console.log('AuthorizedCallback state and nonce validated, returning access token');
                }
            }
        }


        if (authResponseIsValid) {
            this.SetAuthorizationData(token, id_token);
        }
    }

    public Logoff() {
        let authorizationUrl = this.authorityUrl + '/connect/endsession';
        let id_token_hint = this.storage.retrieve('authorizationDataIdToken');
        let post_logout_redirect_uri = location.origin + '/';

        let url =
            authorizationUrl + '?' +
            'id_token_hint=' + encodeURI(id_token_hint) + '&' +
            'post_logout_redirect_uri=' + encodeURI(post_logout_redirect_uri);

        this.ResetAuthorizationData();

        // emit observable
        this.authenticationSource.next(false);
        window.location.href = url;
    }

    public HandleError(error: any) {
        console.log(error);
        if (error.status == 403) {
            this._router.navigate(['/Forbidden']);
        }
        else if (error.status == 401) {
            // this.ResetAuthorizationData();
            this._router.navigate(['/Unauthorized']);
        }
    }

    private urlBase64Decode(str: string) {
        let output = str.replace('-', '+').replace('_', '/');
        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += '==';
                break;
            case 3:
                output += '=';
                break;
            default:
                throw 'Illegal base64url string!';
        }

        return window.atob(output);
    }

    private getDataFromToken(token: any) {
        let data = {};
        if (typeof token !== 'undefined') {
            let encoded = token.split('.')[1];
            data = JSON.parse(this.urlBase64Decode(encoded));
        }

        return data;
    }

    //private retrieve(key: string): any {
    //    let item = this.storage.getItem(key);

    //    if (item && item !== 'undefined') {
    //        return JSON.parse(this.storage.getItem(key));
    //    }

    //    return;
    //}

    //private store(key: string, value: any) {
    //    this.storage.setItem(key, JSON.stringify(value));
    //}

    private getUserData = (): Observable<string[]> => {
        this.setHeaders();
        if (this.authorityUrl === '')
            this.authorityUrl = this.storage.retrieve('IdentityUrl');

        return this._http.get(this.authorityUrl + '/connect/userinfo', {
            headers: this.headers,
            body: ''
        }).map(res => res.json());
    }

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');

        let token = this.GetToken();

        if (token !== '') {
            this.headers.append('Authorization', 'Bearer ' + token);
        }
    }
}