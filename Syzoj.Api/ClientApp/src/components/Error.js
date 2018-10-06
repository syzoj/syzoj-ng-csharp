import React, { Component } from 'react'

export class ErrorList extends Component {
    render() {
        var errors = this.props.errors || []
        if(!Array.isArray(errors))
            errors = [errors]
        return <ul>
            {errors.map((e, i) => <ErrorMessage key={i} message={e.toString()} />)}
        </ul>
    }
}

export class ErrorMessage extends Component {
    render() {
        return <li>Error: {this.props.message}</li>
    }
}