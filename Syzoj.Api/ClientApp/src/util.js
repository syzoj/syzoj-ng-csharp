import config from './config'

export function request(url, method, request) {
    return fetch(config.api + url, {
        method: method,
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        body: request ? JSON.stringify(request) : null,
    }).then((response) => {
        if(response.status >= 500)
            throw "Internal server error: " + url
        return response.json()
    }).then((response) => {
        if(!response.Success)
            throw response.Errors.map(e => e.Message)
        return response.Result
    })
}