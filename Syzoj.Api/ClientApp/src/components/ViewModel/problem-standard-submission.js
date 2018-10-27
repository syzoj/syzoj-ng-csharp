import React, { Component } from 'react'

export default class View extends Component {
    render() {
        return JSON.stringify(this.props.content)
    }
}