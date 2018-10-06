import config from './config'

export function request(url, method, request) {
    return fetch(config.api + url, {
        method: method,
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
    }).then((response) => response.json())
    .then((response) => {
        if(!response.Success)
            throw response.Errors.map(e => e.Message)
    })
}