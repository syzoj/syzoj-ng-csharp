import React, { Component } from 'react'

export default class View extends Component {
    render() {
        return `This is standard problem, data: ${JSON.stringify(this.props.content)}`
    }
}